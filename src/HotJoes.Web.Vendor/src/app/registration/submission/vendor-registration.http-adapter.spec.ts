import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { emptyRegistrationDraft, RegistrationDraft } from "../session/registration-session";
import { HttpVendorRegistrationAdapter } from "./vendor-registration.http-adapter";

describe("HttpVendorRegistrationAdapter", () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [HttpVendorRegistrationAdapter, provideHttpClient(), provideHttpClientTesting()],
  }));

  it("maps the completed session to the approved RegisterVendor request", () => {
    const adapter = TestBed.inject(HttpVendorRegistrationAdapter);
    const http = TestBed.inject(HttpTestingController);
    const draft: RegistrationDraft = {
      ...emptyRegistrationDraft(),
      tradingName: "Hot Joes",
      legalOperatorName: "Hot Joes LLP",
      legalOperatorType: "Limited Liability Partnership",
      companyRegistrationNumber: "AB123456",
      tradingLocation: "Stall",
      addressReference: "addr-stall-hotjoes-001",
      contactName: "John Smith",
      contactEmail: "john@hotjoes.co.uk",
      contactTelephone: "07123 456789",
      website: "https://hotjoes.co.uk",
      businessDescription: "Loaded roast potatoes.",
    };
    let result: unknown;

    adapter.register(draft, {
      authorisedToRegisterBusiness: true,
      informationAccurate: true,
      acceptHotJoesPlatformTerms: true,
    }).subscribe((value) => { result = value; });

    const request = http.expectOne("/vendors");
    expect(request.request.method).toBe("POST");
    expect(request.request.body).toMatchObject({
      legalOperatorType: "limitedLiabilityPartnership",
      tradingCharacteristics: {
        tradingLocation: "stall",
        weeklyOpeningHours: {
          days: [
            { day: "monday", isClosed: false, isOpenAllDay: false, startTime: "09:00:00", endTime: "17:00:00" },
            { day: "tuesday" },
            { day: "wednesday" },
            { day: "thursday" },
            { day: "friday" },
            { day: "saturday" },
            { day: "sunday" },
          ],
        },
      },
      addressResolutionReference: "addr-stall-hotjoes-001",
      registrationDeclarations: {
        authorisedToRegisterBusiness: true,
        informationAccurate: true,
        acceptHotJoesPlatformTerms: true,
      },
    });
    request.flush({ vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", vendorState: "pendingActivation" });
    expect(result).toEqual({ vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", vendorState: "pendingActivation" });
    http.verify();
  });
});
