import { InjectionToken } from "@angular/core";
import { Observable } from "rxjs";
import { RegistrationDraft } from "../session/registration-session";

export type RequiredLicenceType =
  | "foodBusinessRegistration"
  | "streetTradingLicence"
  | "lateNightRefreshmentLicence"
  | "premisesLicence"
  | "personalLicenceHolder";

export interface ComplianceDeterminationItem {
  readonly requiredLicenceType: RequiredLicenceType;
  readonly isRequired: boolean;
}

export interface ComplianceDetermination {
  readonly ruleSetVersion: string;
  readonly items: readonly ComplianceDeterminationItem[];
}

export interface LicenceDeterminationPort {
  determine(draft: RegistrationDraft): Observable<ComplianceDetermination>;
}

export const LICENCE_DETERMINATION = new InjectionToken<LicenceDeterminationPort>(
  "LICENCE_DETERMINATION",
);
