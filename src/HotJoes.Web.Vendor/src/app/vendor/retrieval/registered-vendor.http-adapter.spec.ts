import { provideHttpClient } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { HttpRegisteredVendorAdapter } from "./registered-vendor.http-adapter";

describe("HttpRegisteredVendorAdapter", () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      HttpRegisteredVendorAdapter,
      provideHttpClient(),
      provideHttpClientTesting(),
    ],
  }));

  it("AI-WEB-003 retrieves the persisted Vendor through the approved boundary", () => {
    const adapter = TestBed.inject(HttpRegisteredVendorAdapter);
    const http = TestBed.inject(HttpTestingController);
    let result: unknown;

    adapter.retrieve("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")
      .subscribe((value) => { result = value; });

    const request = http.expectOne(
      "/vendors/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    );
    expect(request.request.method).toBe("GET");
    request.flush(registeredVendor);

    expect(result).toEqual(registeredVendor);
    http.verify();
  });
});

const registeredVendor = {
  vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  registeredAt: "2026-09-25T10:15:30.0000000Z",
  vendorState: "pendingActivation",
  tradingPreference: "acceptingOrders",
  tradingName: "Browser Vendor demonstration",
  legalOperatorType: "soleTrader",
  legalOperatorName: "Browser Operator demonstration",
  companyRegistrationNumber: null,
  tradingCharacteristics: {
    tradingLocation: "restaurant",
    weeklyOpeningHours: {
      days: [
        { day: "monday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
        { day: "tuesday", isClosed: false, isOpenAllDay: false, startTime: "09:00:00", endTime: "17:00:00" },
        { day: "wednesday", isClosed: false, isOpenAllDay: false, startTime: "10:00:00", endTime: "18:00:00" },
        { day: "thursday", isClosed: false, isOpenAllDay: false, startTime: "11:00:00", endTime: "19:00:00" },
        { day: "friday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
        { day: "saturday", isClosed: false, isOpenAllDay: true, startTime: null, endTime: null },
        { day: "sunday", isClosed: true, isOpenAllDay: false, startTime: null, endTime: null },
      ],
    },
    serviceIncludesHotFood: true,
    alcoholService: true,
  },
  primaryContact: {
    contactName: "Browser Contact demonstration",
    contactEmail: "browser-demonstration@example.test",
    contactTelephone: "+447123456789",
  },
  canonicalAddressId: "canonical-blackheath-001",
  businessAddressSnapshot: {
    addressLine1: "Blackheath Avenue",
    addressLine2: null,
    addressLine3: null,
    postTown: "London",
    postcode: "SE10 8XJ",
    county: null,
    recipientOrOrganisationName: "Hot Joes",
  },
  foodRegistrationAuthority: "Royal Borough of Greenwich",
  primaryTradingAuthority: null,
  website: null,
  businessDescription: null,
};
