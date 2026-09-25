import { TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { TradingCharacteristicsStep } from "./trading-characteristics-step";
import { Subject } from "rxjs";
import {
  ComplianceDetermination,
  LICENCE_DETERMINATION,
  LicenceDeterminationPort,
} from "../compliance/licence-determination.port";

describe("TradingCharacteristicsStep", () => {
  let response: Subject<ComplianceDetermination>;
  let determination: LicenceDeterminationPort;

  beforeEach(async () => TestBed.configureTestingModule({
    imports: [TradingCharacteristicsStep],
    providers: [
      RegistrationSessionStore,
      provideRouter([]),
      { provide: LICENCE_DETERMINATION, useValue: { determine: vi.fn() } },
    ],
  }).compileComponents().then(() => {
    response = new Subject<ComplianceDetermination>();
    determination = TestBed.inject(LICENCE_DETERMINATION);
    vi.mocked(determination.determine).mockReturnValue(response);
  }));

  it("AI-WEB-005 exposes the complete approved Step 4 controls", () => {
    const fixture = TestBed.createComponent(TradingCharacteristicsStep);
    fixture.detectChanges();
    const ids = [...fixture.nativeElement.querySelectorAll("input, textarea")].map((control: HTMLInputElement) => control.id);
    expect(ids).toContain("opening-start");
    expect(ids).toContain("opening-end");
    expect(ids).toContain("day-start-0");
    expect(ids).toContain("day-end-6");
    expect(ids).toContain("website");
    expect(ids).toContain("business-description");
    expect(fixture.nativeElement.textContent).toContain("Customise Hours");
    expect(fixture.nativeElement.textContent).toContain("Add Licence");
    expect(fixture.nativeElement.textContent).toContain("Add Website");
    expect(fixture.nativeElement.textContent).toContain("Add Business Description");
    expect(fixture.nativeElement.querySelectorAll("button[type='submit']")).toHaveLength(0);
  });

  it("shows the mockup defaults while accepting No", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    expect(component.form.controls.serviceIncludesHotFood.value).toBe(true);
    expect(component.form.controls.alcoholService.value).toBe(true);
    component.choose("serviceIncludesHotFood", false);
    component.choose("alcoholService", false);
    expect(component.form.controls.serviceIncludesHotFood.valid).toBe(true);
    expect(component.form.controls.alcoholService.valid).toBe(true);
  });

  it("refreshes determination when either service choice changes and cannot confirm a stale result", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    const router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    store.start();
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;

    component.choose("serviceIncludesHotFood", false);
    expect(determination.determine).toHaveBeenCalledTimes(2);
    expect(store.draft()?.serviceIncludesHotFood).toBe(false);
    component.confirmDeterminationAndContinue();
    expect(router.navigate).not.toHaveBeenCalled();

    component.choose("alcoholService", false);
    expect(determination.determine).toHaveBeenCalledTimes(3);
    expect(store.draft()?.alcoholService).toBe(false);
  });

  it("refreshes determination after everyday or customised hours are committed", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    component.form.patchValue({ everydayStart: "18:00", everydayEnd: "02:00" });

    component.applyEveryday();
    expect(determination.determine).toHaveBeenCalledTimes(2);

    component.form.controls.weeklyOpeningHours.at(0).patchValue({ startTime: "19:00", endTime: "01:00" });
    component.saveCustomHours({ close: vi.fn() } as unknown as HTMLDialogElement);
    expect(determination.determine).toHaveBeenCalledTimes(3);
  });

  it("accepts an opening interval that crosses midnight", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    component.form.patchValue({ everydayStart: "18:00", everydayEnd: "02:00" });
    component.applyEveryday();
    expect(component.form.controls.weeklyOpeningHours.controls.every((day) => day.value.startTime === "18:00" && day.value.endTime === "02:00")).toBe(true);
    expect(component.form.controls.weeklyOpeningHours.valid).toBe(true);
  });

  it("supports closed, open-all-day and timed daily states", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    component.toggleDayState(0, "closed", true);
    component.toggleDayState(1, "openAllDay", true);
    expect(component.form.controls.weeklyOpeningHours.at(0).getRawValue()).toMatchObject({ isClosed: true, isOpenAllDay: false, startTime: null, endTime: null });
    expect(component.form.controls.weeklyOpeningHours.at(1).getRawValue()).toMatchObject({ isClosed: false, isOpenAllDay: true, startTime: null, endTime: null });
    expect(component.form.controls.weeklyOpeningHours.valid).toBe(true);
  });

  it("keeps All Day and Closed mutually exclusive", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    component.toggleDayState(0, "openAllDay", true);
    component.toggleDayState(0, "closed", true);
    expect(component.form.controls.weeklyOpeningHours.at(0).getRawValue()).toMatchObject({ isClosed: true, isOpenAllDay: false });
  });

  it("rejects equal timed values and directs the user to Open all day", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    component.form.controls.weeklyOpeningHours.at(0).patchValue({ startTime: "09:00", endTime: "09:00" });
    expect(component.form.controls.weeklyOpeningHours.at(0).hasError("useOpenAllDay")).toBe(true);
  });

  it("obtains and presents the complete determination before leaving Step 4", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    const router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    store.start();
    store.updateBusinessIdentity({ tradingName: "Hot Joes", legalOperatorName: "Hot Joes Ltd", legalOperatorType: "Limited Company", companyRegistrationNumber: "SC123456" });
    store.changeTradingLocation("Stall", true);
    store.selectAddress("addr-stall-hotjoes-001", ["Greenwich Market"]);
    const fixture = TestBed.createComponent(TradingCharacteristicsStep);
    const component = fixture.componentInstance;
    component.form.patchValue({
      everydayStart: "18:00",
      everydayEnd: "02:00",
      serviceIncludesHotFood: true,
      alcoholService: false,
      licenceDetailsRequired: true,
      website: "https://hotjoes.example",
      businessDescription: " Late-night food. ",
    });
    component.applyEveryday();
    vi.mocked(determination.determine).mockClear();
    component.choose("serviceIncludesHotFood", true);
    expect(determination.determine).toHaveBeenCalledOnce();
    expect(router.navigate).not.toHaveBeenCalled();

    response.next(completeDetermination);
    fixture.detectChanges();

    expect(store.draft()).toMatchObject({ serviceIncludesHotFood: true, alcoholService: false, licenceDetailsRequired: true, website: "https://hotjoes.example", businessDescription: "Late-night food." });
    expect(store.draft()?.weeklyOpeningHours).toHaveLength(7);
    expect(store.draft()?.weeklyOpeningHours[6]).toMatchObject({ day: "Sunday", isClosed: false, isOpenAllDay: false, startTime: "18:00", endTime: "02:00" });
    expect(store.determinationReview()?.determination).toEqual(completeDetermination);
    expect(fixture.nativeElement.textContent).toContain("Food Business Registration");
    expect(fixture.nativeElement.textContent).toContain("Street Trading Licence");
    expect(fixture.nativeElement.textContent).toContain("Late Night Refreshment Licence");
    expect(fixture.nativeElement.textContent).toContain("Premises Licence");
    expect(fixture.nativeElement.textContent).toContain("Personal Licence Holder");
    expect(router.navigate).not.toHaveBeenCalled();

    component.confirmDeterminationAndContinue();
    expect(router.navigate).toHaveBeenCalledWith(["/registration/review"]);
  });

  it("renders an asynchronously returned determination without another form interaction", async () => {
    const fixture = TestBed.createComponent(TradingCharacteristicsStep);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain("Checking required licence types");
    expect(fixture.nativeElement.textContent).not.toContain("Required licence types");

    response.next(completeDetermination);
    await fixture.whenStable();

    expect(fixture.nativeElement.textContent).not.toContain("Checking required licence types");
    expect(fixture.nativeElement.textContent).toContain("Required licence types");
    expect(fixture.nativeElement.textContent).toContain("Confirm and continue");
  });

  it("VR-DETERMINATION-013 and 018 retains the draft and prevents progression on controlled failure", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    const router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    store.start();
    store.updateBusinessIdentity({ tradingName: "Hot Joes", legalOperatorName: "Hot Joes Ltd", legalOperatorType: "Limited Company", companyRegistrationNumber: "SC123456" });
    store.changeTradingLocation("Restaurant", true);
    store.selectAddress("addr-unsupported-001", ["Outside supported coverage"]);
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;

    component.choose("serviceIncludesHotFood", true);
    response.error({ status: 409, error: { code: "complianceDeterminationUnsupported", message: "This location is outside supported coverage." } });

    expect(router.navigate).not.toHaveBeenCalled();
    expect(store.draft()?.addressReference).toBe("addr-unsupported-001");
    expect(store.determinationReview()).toBeNull();
    expect(component.determinationError).toContain("outside supported coverage");
    expect(component.determining).toBe(false);
  });

  it("rejects a non-HTTPS website", () => {
    const component = TestBed.createComponent(TradingCharacteristicsStep).componentInstance;
    component.form.controls.website.setValue("http://hotjoes.example");
    expect(component.form.controls.website.invalid).toBe(true);
  });

  it("saves website and business description into the registration session for Step 5", () => {
    const fixture = TestBed.createComponent(TradingCharacteristicsStep);
    const store = TestBed.inject(RegistrationSessionStore);
    const websiteDialog = { close: vi.fn() } as unknown as HTMLDialogElement;
    const descriptionDialog = { close: vi.fn() } as unknown as HTMLDialogElement;
    fixture.componentInstance.form.patchValue({
      website: "https://hotjoes.example",
      businessDescription: "Late-night food from our Greenwich kitchen.",
    });
    fixture.componentInstance.saveWebsite(websiteDialog);
    fixture.componentInstance.saveBusinessDescription(descriptionDialog);
    fixture.detectChanges();

    expect(store.draft()?.website).toBe("https://hotjoes.example");
    expect(store.draft()?.businessDescription).toBe("Late-night food from our Greenwich kitchen.");
    expect(websiteDialog.close).toHaveBeenCalledOnce();
    expect(descriptionDialog.close).toHaveBeenCalledOnce();
    expect(fixture.nativeElement.textContent).toContain("https://hotjoes.example");
    expect(fixture.nativeElement.textContent).toContain("Late-night food from our Greenwich kitchen.");
  });
});

const completeDetermination: ComplianceDetermination = {
  ruleSetVersion: "epic-1-v1",
  items: [
    { requiredLicenceType: "foodBusinessRegistration", isRequired: true },
    { requiredLicenceType: "streetTradingLicence", isRequired: true },
    { requiredLicenceType: "lateNightRefreshmentLicence", isRequired: true },
    { requiredLicenceType: "premisesLicence", isRequired: false },
    { requiredLicenceType: "personalLicenceHolder", isRequired: false },
  ],
};
