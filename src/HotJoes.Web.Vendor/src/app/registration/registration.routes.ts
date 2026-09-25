import { Routes } from "@angular/router";
import { BusinessIdentityStep } from "./business/business-identity-step";
import { TradingLocationStep } from "./location/trading-location-step";
import { LegalOperatorStep } from "./operator/legal-operator-step";
import { TradingCharacteristicsStep } from "./characteristics/trading-characteristics-step";
import { RegistrationReviewStep } from "./review/registration-review-step";
import { RegistrationConfirmationStep } from "./confirmation/registration-confirmation-step";
import { CommunityJoinedStep } from "./community/community-joined-step";
import { CommunityNotJoinedStep } from "./community/community-not-joined-step";
import {
  RegistrationJourney,
  RegistrationStart,
} from "./journey/registration-journey";

export const registrationRoutes: Routes = [
  {
    path: "",
    component: RegistrationJourney,
    children: [
      { path: "start", component: RegistrationStart },
      { path: "business", component: BusinessIdentityStep },
      { path: "location", component: TradingLocationStep },
      { path: "operator", component: LegalOperatorStep },
      { path: "characteristics", component: TradingCharacteristicsStep },
      { path: "review", component: RegistrationReviewStep },
      { path: "confirmation", component: RegistrationConfirmationStep },
      { path: "community-joined", component: CommunityJoinedStep },
      { path: "community-not-joined", component: CommunityNotJoinedStep },
      { path: "", pathMatch: "full", redirectTo: "start" },
      { path: "**", redirectTo: "start" },
    ],
  },
];
