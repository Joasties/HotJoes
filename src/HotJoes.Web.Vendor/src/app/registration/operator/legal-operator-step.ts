import { Component, inject } from "@angular/core";
import { Router } from "@angular/router";
import { LegalOperatorType } from "../session/registration-session";
import { RegistrationSessionStore } from "../session/registration-session.store";

interface LegalOperatorOption {
  readonly value: LegalOperatorType;
  readonly image: string;
  readonly description: string;
}

@Component({
  selector: "app-legal-operator-step",
  templateUrl: "./legal-operator-step.html",
  styleUrl: "./legal-operator-step.scss",
})
export class LegalOperatorStep {
  readonly #store = inject(RegistrationSessionStore);
  readonly #router = inject(Router);
  readonly options: readonly LegalOperatorOption[] = [
    {
      value: "Sole Trader",
      image: "sole-trader.png",
      description:
        "You run the business as an individual and are personally responsible for it.",
    },
    {
      value: "General Partnership",
      image: "general-partnership.png",
      description:
        "You and one or more partners share ownership and responsibility for the business.",
    },
    {
      value: "Limited Company",
      image: "limited-company.png",
      description: "The business is a separate legal entity, limited by shares.",
    },
    {
      value: "Limited Liability Partnership",
      image: "limited-liability-partnership.png",
      description: "A partnership structure where partners have limited liability.",
    },
    {
      value: "Charitable Community Group",
      image: "charitable-community-group.png",
      description: "A community or voluntary group running a food business.",
    },
    {
      value: "Charitable Incorporated Organisation",
      image: "charitable-incorporated-organisation.png",
      description: "A registered charity with its own legal identity.",
    },
  ];
  selected: LegalOperatorType | null;

  constructor() {
    this.#store.start();
    this.selected = this.#store.draft()?.legalOperatorType ?? null;
  }

  choose(next: LegalOperatorType): void {
    this.#store.selectLegalOperatorType(next);
    this.selected = next;
    void this.#router.navigate(["/registration/business"]);
  }

  back(): void {
    void this.#router.navigate(["/registration/location"]);
  }
}
