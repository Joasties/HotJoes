import { TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { TradingLocationStep } from "./trading-location-step";

describe("TradingLocationStep", () => {
  beforeEach(async () =>
    TestBed.configureTestingModule({
      imports: [TradingLocationStep],
      providers: [RegistrationSessionStore, provideRouter([])],
    }).compileComponents(),
  );

  it("AI-UX-005 renders exactly the HJ-104 controlled locations as labelled radios", () => {
    const fixture = TestBed.createComponent(TradingLocationStep);
    fixture.detectChanges();
    const radios = [
      ...fixture.nativeElement.querySelectorAll("input[type=radio]"),
    ] as HTMLInputElement[];
    expect(radios.map((radio) => radio.value)).toEqual([
      "Restaurant",
      "Stall",
      "Kitchen",
    ]);
  });

  it("F-002 changes the selected radio without navigating", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    const router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    TestBed.createComponent(TradingLocationStep).componentInstance.choose(
      "Stall",
    );
    expect(store.draft()?.tradingLocation).toBe("Stall");
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it("F-002 exposes a separate Continue action that confirms an unchanged selection", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    const router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    store.start();
    store.changeTradingLocation("Restaurant", true);

    const fixture = TestBed.createComponent(TradingLocationStep);
    fixture.detectChanges();
    const continueButton = [...fixture.nativeElement.querySelectorAll("button")]
      .find((button: HTMLButtonElement) => button.textContent?.trim() === "Continue");

    expect(continueButton).toBeDefined();
    continueButton?.click();
    expect(router.navigate).toHaveBeenCalledWith(["/registration/operator"]);
  });

  it("AI-SESSION-003 preserves dependent state when change is cancelled", () => {
    const store = TestBed.inject(RegistrationSessionStore);
    store.start();
    store.changeTradingLocation("Restaurant", true);
    store.selectAddress("address-42", ["42 Test Street"]);
    vi.spyOn(window, "confirm").mockReturnValue(false);
    TestBed.createComponent(TradingLocationStep).componentInstance.choose(
      "Kitchen",
    );
    expect(store.draft()?.tradingLocation).toBe("Restaurant");
    expect(store.draft()?.addressReference).toBe("address-42");
  });
});
