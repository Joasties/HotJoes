import { TestBed } from "@angular/core/testing";
import { RegistrationSessionStore } from "./registration-session.store";
import { ComplianceDetermination } from "../compliance/licence-determination.port";

const businessIdentity = {
  tradingName: "Hot Joes",
  legalOperatorName: "Joasties Ltd",
  legalOperatorType: "Limited Company" as const,
  companyRegistrationNumber: "SC123456",
};
const weeklyOpeningHours = (["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"] as const).map((day) => ({ day, isClosed: false, isOpenAllDay: false, startTime: "18:00", endTime: "02:00" }));
const determination: ComplianceDetermination = {
  ruleSetVersion: "epic-1-v1",
  items: [
    { requiredLicenceType: "foodBusinessRegistration", isRequired: true },
    { requiredLicenceType: "streetTradingLicence", isRequired: false },
    { requiredLicenceType: "lateNightRefreshmentLicence", isRequired: true },
    { requiredLicenceType: "premisesLicence", isRequired: false },
    { requiredLicenceType: "personalLicenceHolder", isRequired: false },
  ],
};

describe("RegistrationSessionStore", () => {
  let store: RegistrationSessionStore;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [RegistrationSessionStore] });
    store = TestBed.inject(RegistrationSessionStore);
  });

  it("AI-SESSION-001 starts with transient empty client state", () => {
    expect(store.session()).toBeNull();
    store.start();
    expect(store.isActive()).toBe(true);
    expect(store.draft()).toEqual({
      contactName: "",
      contactEmail: "",
      contactTelephone: "",
      tradingName: "",
      legalOperatorName: "",
      legalOperatorType: null,
      companyRegistrationNumber: "",
      tradingLocation: null,
      addressReference: null,
      addressDisplayLines: [],
      weeklyOpeningHours: weeklyOpeningHours.map((hours) => ({ ...hours, startTime: "09:00", endTime: "17:00" })),
      serviceIncludesHotFood: true,
      alcoholService: true,
      licenceDetailsRequired: false,
      website: "",
      businessDescription: "",
    });
  });

  it("AI-SESSION-002 retains user intent while navigating the journey", () => {
    store.start();
    store.updateBusinessIdentity(businessIdentity);
    expect(store.draft()?.tradingName).toBe("Hot Joes");
  });

  it("retains trading characteristics and optional profile information", () => {
    store.start();
    store.updateTradingCharacteristics({
      weeklyOpeningHours,
      serviceIncludesHotFood: true,
      alcoholService: false,
      licenceDetailsRequired: true,
      website: "https://hotjoes.example",
      businessDescription: "Late-night food.",
    });
    expect(store.draft()).toMatchObject({
      weeklyOpeningHours,
      serviceIncludesHotFood: true,
      alcoholService: false,
      licenceDetailsRequired: true,
    });
  });

  it("AI-SESSION-003 preserves state when destructive reset is cancelled", () => {
    store.start();
    store.updateBusinessIdentity(businessIdentity);
    expect(store.reset(false)).toBe(false);
    expect(store.draft()?.tradingName).toBe("Hot Joes");
  });

  it("AI-SESSION-003 clears the complete draft after reset is confirmed", () => {
    store.start();
    store.updateBusinessIdentity(businessIdentity);
    store.changeTradingLocation("Restaurant", true);
    store.selectAddress("address-42", ["42 Test Street"]);
    expect(store.reset(true)).toBe(true);
    expect(store.draft()?.addressReference).toBeNull();
    expect(store.draft()?.tradingName).toBe("");
  });

  it("requires confirmation before a location change discards an address", () => {
    store.start();
    store.changeTradingLocation("Restaurant", true);
    store.selectAddress("address-42", ["42 Test Street"]);
    expect(store.changeTradingLocation("Stall", false)).toBe(false);
    expect(store.draft()?.addressReference).toBe("address-42");
    expect(store.changeTradingLocation("Stall", true)).toBe(true);
    expect(store.draft()?.addressReference).toBeNull();
    expect(store.draft()?.addressDisplayLines).toEqual([]);
  });

  it("AI-SESSION-002 terminates and discards an abandoned session", () => {
    store.start();
    store.abandon();
    expect(store.session()).toBeNull();
  });

  it("VR-DETERMINATION-014 retains only the complete determination and controlling-input fingerprint", () => {
    completeControllingInputs();

    store.recordComplianceDetermination(determination);

    expect(store.determinationReview()?.determination).toEqual(determination);
    expect(store.determinationReview()?.controllingInputFingerprint).toEqual(expect.any(String));
    expect(store.determinationReview()!.controllingInputFingerprint.length).toBeGreaterThan(0);
    expect(store.isDeterminationCurrent()).toBe(true);
  });

  it.each([
    ["Legal Operator Type", () => store.selectLegalOperatorType("General Partnership")],
    ["Trading Location", () => store.changeTradingLocation("Kitchen", true)],
    ["Weekly Opening Hours", () => store.updateTradingCharacteristics({
      weeklyOpeningHours: weeklyOpeningHours.map((hours, index) => index === 0 ? { ...hours, startTime: "19:00" } : hours),
      serviceIncludesHotFood: true,
      alcoholService: false,
      licenceDetailsRequired: false,
      website: "",
      businessDescription: "",
    })],
    ["Service Includes Hot Food", () => store.updateTradingCharacteristics({
      weeklyOpeningHours,
      serviceIncludesHotFood: false,
      alcoholService: false,
      licenceDetailsRequired: false,
      website: "",
      businessDescription: "",
    })],
    ["Alcohol Service", () => store.updateTradingCharacteristics({
      weeklyOpeningHours,
      serviceIncludesHotFood: true,
      alcoholService: true,
      licenceDetailsRequired: false,
      website: "",
      businessDescription: "",
    })],
    ["Address Resolution Reference", () => store.selectAddress("address-99", ["99 New Street"])],
  ])("VR-DETERMINATION-015 invalidates all retained review state when %s changes", (_field, change) => {
    completeControllingInputs();
    store.recordComplianceDetermination(determination);

    change();

    expect(store.determinationReview()).toBeNull();
    expect(store.isDeterminationCurrent()).toBe(false);
  });

  it("preserves determination review when only non-controlling information changes", () => {
    completeControllingInputs();
    store.recordComplianceDetermination(determination);

    store.updatePrimaryContact({
      contactName: "Jordan Smith",
      contactEmail: "jordan@example.test",
      contactTelephone: "020 7946 0123",
    });
    store.updateBusinessIdentity({ ...businessIdentity, tradingName: "A new display name" });

    expect(store.determinationReview()?.determination).toEqual(determination);
    expect(store.isDeterminationCurrent()).toBe(true);
  });

  function completeControllingInputs(): void {
    store.start();
    store.updateBusinessIdentity(businessIdentity);
    store.changeTradingLocation("Restaurant", true);
    store.selectAddress("address-42", ["42 Test Street"]);
    store.updateTradingCharacteristics({
      weeklyOpeningHours,
      serviceIncludesHotFood: true,
      alcoholService: false,
      licenceDetailsRequired: false,
      website: "",
      businessDescription: "",
    });
  }
});
