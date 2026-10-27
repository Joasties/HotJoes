import { provideZonelessChangeDetection } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ActivatedRoute, convertToParamMap } from "@angular/router";
import { Subject } from "rxjs";
import { RegisteredVendorDetailsStep } from "./registered-vendor-details-step";
import {
  REGISTERED_VENDOR_RETRIEVAL,
  RegisteredVendorDetails,
  RegisteredVendorRetrievalPort,
} from "./registered-vendor.port";

describe("RegisteredVendorDetailsStep", () => {
  let fixture: ComponentFixture<RegisteredVendorDetailsStep>;
  let response: Subject<RegisteredVendorDetails>;
  let retrieval: RegisteredVendorRetrievalPort;

  beforeEach(async () => {
    response = new Subject<RegisteredVendorDetails>();
    retrieval = { retrieve: vi.fn().mockReturnValue(response) };

    await TestBed.configureTestingModule({
      imports: [RegisteredVendorDetailsStep],
      providers: [
        provideZonelessChangeDetection(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: convertToParamMap({
                vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
              }),
            },
          },
        },
        { provide: REGISTERED_VENDOR_RETRIEVAL, useValue: retrieval },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisteredVendorDetailsStep);
    fixture.detectChanges();
  });

  it("AI-WEB-003 retrieves by the returned Vendor ID outside Registration Session", () => {
    expect(retrieval.retrieve).toHaveBeenCalledOnce();
    expect(retrieval.retrieve).toHaveBeenCalledWith(
      "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    );
    expect(fixture.nativeElement.textContent).toContain(
      "Retrieving registered Vendor details",
    );
  });

  it("AI-BROWSER-010 presents persisted identity, contact, address and weekly hours", async () => {
    response.next(registeredVendor);
    response.complete();
    await fixture.whenStable();
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(fixture.nativeElement.querySelector("h1")?.textContent)
      .toContain("Registered Vendor details");
    expect(text).toContain("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    expect(text).toContain("Browser Vendor demonstration");
    expect(text).toContain("Browser Operator demonstration");
    expect(text).toContain("browser-demonstration@example.test");
    expect(text).toContain("Blackheath Avenue");
    expect(text).toContain("Monday");
    expect(text).toContain("Closed");
    expect(text).toContain("Tuesday");
    expect(text).toContain("09:00 to 17:00");
    expect(text).toContain("Friday");
    expect(text).toContain("Open all day");
  });

  it("AI-WEB-003 reports retrieval failure without recasting registration", async () => {
    response.error({ status: 503 });
    await fixture.whenStable();
    fixture.detectChanges();

    const alert = fixture.nativeElement.querySelector("[role=alert]");
    expect(alert?.textContent).toContain(
      "Registration succeeded, but the registered details could not be retrieved",
    );
  });
});

const registeredVendor: RegisteredVendorDetails = {
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
