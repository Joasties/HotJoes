import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { RegistrationReviewStep } from "./registration-review-step";
import { Subject } from "rxjs";
import { VENDOR_REGISTRATION, VendorRegistrationPort, VendorRegistrationResult } from "../submission/vendor-registration.port";
import {
  ComplianceDetermination,
  LICENCE_DETERMINATION,
  LicenceDeterminationPort,
} from "../compliance/licence-determination.port";

describe("RegistrationReviewStep", () => {
  let store: RegistrationSessionStore;
  let router: Router;
  let response: Subject<VendorRegistrationResult>;
  let registration: VendorRegistrationPort;
  let determination: LicenceDeterminationPort;
  let determinationResponse: Subject<ComplianceDetermination>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistrationReviewStep],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        RegistrationSessionStore,
        { provide: VENDOR_REGISTRATION, useValue: { register: vi.fn() } },
        { provide: LICENCE_DETERMINATION, useValue: { determine: vi.fn() } },
      ],
    }).compileComponents();
    store = TestBed.inject(RegistrationSessionStore);
    router = TestBed.inject(Router);
    registration = TestBed.inject(VENDOR_REGISTRATION);
    determination = TestBed.inject(LICENCE_DETERMINATION);
    response = new Subject<VendorRegistrationResult>();
    determinationResponse = new Subject<ComplianceDetermination>();
    vi.mocked(registration.register).mockReturnValue(response);
    vi.mocked(determination.determine).mockReturnValue(determinationResponse);
  });

  it("starts every registration declaration switched off", () => {
    const component = TestBed.createComponent(RegistrationReviewStep).componentInstance;
    expect(component.declarations.getRawValue()).toEqual({ authorised: false, accurate: false, termsAccepted: false });
    expect(component.canSubmit).toBe(false);
  });

  it("shows the contact and complete Monday-to-Sunday opening-hours summary", () => {
    store.start();
    store.updatePrimaryContact({ contactName: "John Smith", contactTelephone: "07123 456789", contactEmail: "john@hotjoes.co.uk" });
    const fixture = TestBed.createComponent(RegistrationReviewStep);
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain("Primary contact details");
    expect(text).toContain("john@hotjoes.co.uk");
    expect(text).toContain("Monday");
    expect(text).toContain("Sunday");
    expect(text).toContain("09:00 to 17:00");
  });

  it("enables submission only after all declarations and a current determination review", () => {
    const component = TestBed.createComponent(RegistrationReviewStep).componentInstance;
    component.declarations.setValue({ authorised: true, accurate: true, termsAccepted: true });
    expect(component.canSubmit).toBe(false);
    prepareReviewedDraft();
    expect(component.canSubmit).toBe(true);
  });

  it("returns to Step 4 from Back", () => {
    const navigate = vi.spyOn(router, "navigate").mockResolvedValue(true);
    TestBed.createComponent(RegistrationReviewStep).componentInstance.back();
    expect(navigate).toHaveBeenCalledWith(["/registration/characteristics"]);
  });

  it("VR-DETERMINATION-016 refreshes first and submits once only when the result is unchanged", () => {
    const navigate = vi.spyOn(router, "navigate").mockResolvedValue(true);
    prepareReviewedDraft();
    const component = TestBed.createComponent(RegistrationReviewStep).componentInstance;
    component.declarations.setValue({ authorised: true, accurate: true, termsAccepted: true });
    component.submit();
    component.submit();
    expect(determination.determine).toHaveBeenCalledOnce();
    expect(registration.register).not.toHaveBeenCalled();
    expect(component.submitting).toBe(true);

    determinationResponse.next(reviewedDetermination);

    expect(registration.register).toHaveBeenCalledOnce();
    expect(component.submitting).toBe(true);

    response.next({ vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", vendorState: "pendingActivation" });
    expect(component.result?.vendorId).toBe("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    expect(component.submitting).toBe(false);
    expect(navigate).toHaveBeenCalledWith(["/registration/confirmation"]);
  });

  it.each([
    ["Rule Set Version", { ...reviewedDetermination, ruleSetVersion: "epic-1-v2" }],
    ["item set", {
      ...reviewedDetermination,
      items: reviewedDetermination.items.map((item) => item.requiredLicenceType === "premisesLicence" ? { ...item, isRequired: true } : item),
    }],
  ])("VR-DETERMINATION-017 changed %s requires renewed review and prevents registration", (_change, refreshed) => {
    const navigate = vi.spyOn(router, "navigate").mockResolvedValue(true);
    prepareReviewedDraft();
    const component = TestBed.createComponent(RegistrationReviewStep).componentInstance;
    component.declarations.setValue({ authorised: true, accurate: true, termsAccepted: true });

    component.submit();
    determinationResponse.next(refreshed as ComplianceDetermination);

    expect(registration.register).not.toHaveBeenCalled();
    expect(store.determinationReview()?.determination).toEqual(refreshed);
    expect(component.submitting).toBe(false);
    expect(component.submissionError).toContain("review");
    expect(navigate).toHaveBeenCalledWith(["/registration/characteristics"]);
  });

  it("VR-DETERMINATION-018 retains the draft when the fresh determination fails", () => {
    prepareReviewedDraft();
    const component = TestBed.createComponent(RegistrationReviewStep).componentInstance;
    component.declarations.setValue({ authorised: true, accurate: true, termsAccepted: true });

    component.submit();
    determinationResponse.error({ status: 503, error: { code: "complianceDeterminationTemporarilyUnavailable", message: "Determination is temporarily unavailable." } });

    expect(registration.register).not.toHaveBeenCalled();
    expect(store.draft()?.tradingName).toBe("Hot Joes");
    expect(store.determinationReview()?.determination).toEqual(reviewedDetermination);
    expect(component.submitting).toBe(false);
    expect(component.submissionError).toContain("temporarily unavailable");
  });

  it("renders a registration failure and leaves the submitting state without another interaction", async () => {
    prepareReviewedDraft();
    const fixture = TestBed.createComponent(RegistrationReviewStep);
    fixture.componentInstance.declarations.setValue({ authorised: true, accurate: true, termsAccepted: true });
    fixture.detectChanges();

    fixture.componentInstance.submit();
    determinationResponse.next(reviewedDetermination);
    response.error({ status: 503, error: { message: "Registration is temporarily unavailable." } });
    await fixture.whenStable();

    expect(fixture.componentInstance.submitting).toBe(false);
    expect(fixture.nativeElement.textContent).toContain("Registration is temporarily unavailable.");
    expect(fixture.nativeElement.textContent).toContain("Submit Registration");
    expect(fixture.nativeElement.textContent).not.toContain("Submitting…");
  });

  function prepareReviewedDraft(): void {
    store.start();
    store.updateBusinessIdentity({ tradingName: "Hot Joes", legalOperatorName: "Hot Joes Ltd", legalOperatorType: "Limited Company", companyRegistrationNumber: "SC123456" });
    store.changeTradingLocation("Restaurant", true);
    store.selectAddress("addr-restaurant-hotjoes-001", ["1 Test Street"]);
    store.recordComplianceDetermination(reviewedDetermination);
  }
});

const reviewedDetermination: ComplianceDetermination = {
  ruleSetVersion: "epic-1-v1",
  items: [
    { requiredLicenceType: "foodBusinessRegistration", isRequired: true },
    { requiredLicenceType: "streetTradingLicence", isRequired: false },
    { requiredLicenceType: "lateNightRefreshmentLicence", isRequired: false },
    { requiredLicenceType: "premisesLicence", isRequired: false },
    { requiredLicenceType: "personalLicenceHolder", isRequired: false },
  ],
};
