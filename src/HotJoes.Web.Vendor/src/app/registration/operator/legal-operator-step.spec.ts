import { TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { LegalOperatorStep } from "./legal-operator-step";

describe("LegalOperatorStep", () => {
  beforeEach(async () =>
    TestBed.configureTestingModule({
      imports: [LegalOperatorStep],
      providers: [RegistrationSessionStore, provideRouter([])],
    }).compileComponents(),
  );

  it("AI-UX-005 renders exactly the six controlled legal operator types", () => {
    const fixture = TestBed.createComponent(LegalOperatorStep);
    fixture.detectChanges();
    const radios = [
      ...fixture.nativeElement.querySelectorAll("input[type=radio]"),
    ] as HTMLInputElement[];
    expect(radios.map((radio) => radio.value)).toEqual([
      "Sole Trader",
      "General Partnership",
      "Limited Company",
      "Limited Liability Partnership",
      "Charitable Community Group",
      "Charitable Incorporated Organisation",
    ]);
  });

  it("commits selection and continues to Business Identity", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    const router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    TestBed.createComponent(LegalOperatorStep).componentInstance.choose(
      "Limited Company",
    );
    expect(store.draft()?.legalOperatorType).toBe("Limited Company");
    expect(router.navigate).toHaveBeenCalledWith(["/registration/business"]);
  });

  it("restores a previously selected operator type", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    store.start();
    store.selectLegalOperatorType("General Partnership");
    const fixture = TestBed.createComponent(LegalOperatorStep);
    fixture.detectChanges();
    expect(fixture.componentInstance.selected).toBe("General Partnership");
    expect(fixture.nativeElement.querySelector("input:checked")?.value).toBe(
      "General Partnership",
    );
  });

  it("returns to Trading Location from Back", () => {
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, "navigate").mockResolvedValue(true);

    TestBed.createComponent(LegalOperatorStep).componentInstance.back();

    expect(navigate).toHaveBeenCalledWith(["/registration/location"]);
  });
});
