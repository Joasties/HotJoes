import { Component, inject } from "@angular/core";
import { Router } from "@angular/router";
import { TradingLocation } from "../session/registration-session";
import { RegistrationSessionStore } from "../session/registration-session.store";

@Component({
  selector: "app-trading-location-step",
  templateUrl: "./trading-location-step.html",
  styleUrl: "./trading-location-step.scss",
})
export class TradingLocationStep {
  readonly #store = inject(RegistrationSessionStore);
  readonly #router = inject(Router);
  readonly options = [
    {
      value: "Restaurant" as const,
      title: "Restaurant",
      description:
        "A customer-facing permanent premises, including a café, takeaway or food market hall.",
      symbol: "♨",
    },
    {
      value: "Stall" as const,
      title: "Stall",
      description:
        "A mobile food unit, market stall, kiosk or temporary trading space.",
      symbol: "▱",
    },
    {
      value: "Kitchen" as const,
      title: "Kitchen",
      description:
        "A delivery-only dark, ghost or home kitchen without customer-facing premises.",
      symbol: "⌂",
    },
  ];
  selected: TradingLocation | null;

  constructor() {
    this.#store.start();
    this.selected = this.#store.draft()?.tradingLocation ?? null;
  }

  choose(next: TradingLocation): void {
    const draft = this.#store.draft();
    const changesPopulatedLocation =
      draft?.tradingLocation !== null && draft?.tradingLocation !== next;
    const confirmed =
      !changesPopulatedLocation ||
      window.confirm(
        "Changing trading location restarts the location-dependent part of registration and clears the selected business address. Continue?",
      );
    if (!this.#store.changeTradingLocation(next, confirmed)) return;
    this.selected = next;
    void this.#router.navigate(["/registration/operator"]);
  }
}
