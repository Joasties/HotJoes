import { TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { BusinessIdentityStep } from "./business-identity-step";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { of } from "rxjs";
import { ADDRESS_SEARCH, AddressSearchPort } from "../address/address-search.port";

const addressSearch: AddressSearchPort = {
  search: (query, tradingLocation) => of(
    query.toLocaleLowerCase("en-GB").includes("greenwich") && tradingLocation === "Stall"
      ? [{ reference: "addr-stall-hotjoes-001", lines: ["Hot Joes Stall", "Greenwich Market", "London", "SE10 9HZ"] }]
      : query.toLocaleLowerCase("en-GB").includes("blackheath") && tradingLocation === "Restaurant"
        ? [{ reference: "addr-restaurant-hotjoes-001", lines: ["Hot Joes", "Blackheath Avenue", "London", "SE10 8XJ"] }]
        : [],
  ),
};

describe("BusinessIdentityStep", () => {
  beforeEach(async () => TestBed.configureTestingModule({ imports: [BusinessIdentityStep], providers: [RegistrationSessionStore, provideRouter([]), { provide: ADDRESS_SEARCH, useValue: addressSearch }] }).compileComponents());

  it("AI-WEB-005 exposes complete Step 3 controls", () => {
    const fixture = TestBed.createComponent(BusinessIdentityStep); fixture.detectChanges();
    expect([...fixture.nativeElement.querySelectorAll("input")].map((input: HTMLInputElement) => input.id)).toEqual(["contact-name", "contact-telephone", "contact-email", "trading-name", "legal-name", "address-query"]);
  });

  it("requires a company number only for applicable operator types", () => {
    const store = TestBed.inject(RegistrationSessionStore); store.start(); store.selectLegalOperatorType("Sole Trader");
    let component = TestBed.createComponent(BusinessIdentityStep).componentInstance;
    expect(component.requiresCompanyNumber).toBe(false);
    store.selectLegalOperatorType("Limited Company"); component = TestBed.createComponent(BusinessIdentityStep).componentInstance;
    expect(component.requiresCompanyNumber).toBe(true);
    expect(component.form.controls.companyRegistrationNumber.hasError("required")).toBe(true);
    component.form.controls.companyRegistrationNumber.setValue("SC1234567");
    expect(component.form.controls.companyRegistrationNumber.hasError("maxlength")).toBe(true);
  });

  it("searches within the selected location and persists the address", () => {
    const store = TestBed.inject(RegistrationSessionStore); store.start(); store.changeTradingLocation("Stall", true); store.selectLegalOperatorType("General Partnership");
    const component = TestBed.createComponent(BusinessIdentityStep).componentInstance;
    component.form.controls.addressQuery.setValue("Greenwich"); component.searchAddress();
    expect(component.addressResults).toHaveLength(1); component.selectAddress(component.addressResults[0]);
    expect(store.draft()?.addressReference).toBe("addr-stall-hotjoes-001");
    expect(store.draft()?.addressDisplayLines).toContain("Greenwich Market");
  });

  it("commits complete Step 3 state and continues to Step 4", () => {
    const store = TestBed.inject(RegistrationSessionStore); const router = TestBed.inject(Router); vi.spyOn(router, "navigate").mockResolvedValue(true);
    store.start(); store.changeTradingLocation("Restaurant", true); store.selectLegalOperatorType("Limited Company");
    const component = TestBed.createComponent(BusinessIdentityStep).componentInstance;
    component.form.patchValue({ contactName: "Joe Guest", contactTelephone: "020 7946 0123", contactEmail: "joe@example.com", tradingName: "Hot Joes", legalOperatorName: "Joasties Ltd", companyRegistrationNumber: "sc123456", addressQuery: "Blackheath" });
    component.searchAddress(); component.selectAddress(component.addressResults[0]); component.continue();
    expect(store.draft()).toMatchObject({ contactName: "Joe Guest", contactEmail: "joe@example.com", companyRegistrationNumber: "SC123456", addressReference: "addr-restaurant-hotjoes-001" });
    expect(router.navigate).toHaveBeenCalledWith(["/registration/characteristics"]);
  });
});
