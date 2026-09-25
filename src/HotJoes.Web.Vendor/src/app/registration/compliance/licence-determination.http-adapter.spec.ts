import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { emptyRegistrationDraft, RegistrationDraft } from "../session/registration-session";
import { HttpLicenceDeterminationAdapter } from "./licence-determination.http-adapter";

describe("HttpLicenceDeterminationAdapter", () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [HttpLicenceDeterminationAdapter, provideHttpClient(), provideHttpClientTesting()],
  }));

  it("VR-DETERMINATION-001 sends exactly the six controlling inputs", () => {
    const adapter = TestBed.inject(HttpLicenceDeterminationAdapter);
    const http = TestBed.inject(HttpTestingController);
    const draft: RegistrationDraft = {
      ...emptyRegistrationDraft(),
      tradingName: "Must not be sent",
      legalOperatorName: "Must not be sent",
      legalOperatorType: "Limited Company",
      tradingLocation: "Stall",
      addressReference: "addr-stall-hotjoes-001",
      contactName: "Must not be sent",
      website: "https://must-not-be-sent.example",
      serviceIncludesHotFood: true,
      alcoholService: false,
    };

    adapter.determine(draft).subscribe();

    const request = http.expectOne("/vendor-registration/required-licence-types");
    expect(request.request.method).toBe("POST");
    expect(Object.keys(request.request.body)).toEqual([
      "legalOperatorType",
      "tradingLocation",
      "weeklyOpeningHours",
      "serviceIncludesHotFood",
      "alcoholService",
      "addressResolutionReference",
    ]);
    expect(request.request.body).toEqual({
      legalOperatorType: "limitedCompany",
      tradingLocation: "stall",
      weeklyOpeningHours: {
        days: draft.weeklyOpeningHours.map((hours) => ({
          day: hours.day.toLocaleLowerCase("en-GB"),
          isClosed: hours.isClosed,
          isOpenAllDay: hours.isOpenAllDay,
          startTime: hours.startTime && `${hours.startTime}:00`,
          endTime: hours.endTime && `${hours.endTime}:00`,
        })),
      },
      serviceIncludesHotFood: true,
      alcoholService: false,
      addressResolutionReference: "addr-stall-hotjoes-001",
    });
    request.flush(completeDetermination);
    http.verify();
  });

  it("VR-DETERMINATION-014 retains the complete immutable wire result", () => {
    const adapter = TestBed.inject(HttpLicenceDeterminationAdapter);
    const http = TestBed.inject(HttpTestingController);
    const draft: RegistrationDraft = {
      ...emptyRegistrationDraft(),
      legalOperatorType: "Limited Company",
      tradingLocation: "Kitchen",
      addressReference: "addr-kitchen-hotjoes-001",
    };
    let actual: unknown;

    adapter.determine(draft).subscribe((value) => { actual = value; });
    http.expectOne("/vendor-registration/required-licence-types")
      .flush(completeDetermination);

    expect(actual).toEqual(completeDetermination);
    expect(Object.isFrozen(actual)).toBe(true);
    expect(Object.isFrozen((actual as typeof completeDetermination).items)).toBe(true);
  });
});

const completeDetermination = {
  ruleSetVersion: "epic-1-v1",
  items: [
    { requiredLicenceType: "foodBusinessRegistration", isRequired: true },
    { requiredLicenceType: "streetTradingLicence", isRequired: true },
    { requiredLicenceType: "lateNightRefreshmentLicence", isRequired: false },
    { requiredLicenceType: "premisesLicence", isRequired: false },
    { requiredLicenceType: "personalLicenceHolder", isRequired: false },
  ],
} as const;
