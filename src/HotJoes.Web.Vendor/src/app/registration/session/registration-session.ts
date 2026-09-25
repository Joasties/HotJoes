import type { ComplianceDetermination } from "../compliance/licence-determination.port";

export type TradingLocation = "Restaurant" | "Stall" | "Kitchen";
export type TradingDay = "Monday" | "Tuesday" | "Wednesday" | "Thursday" | "Friday" | "Saturday" | "Sunday";

export interface DailyOpeningHours {
  readonly day: TradingDay;
  readonly isClosed: boolean;
  readonly isOpenAllDay: boolean;
  readonly startTime: string | null;
  readonly endTime: string | null;
}

export type LegalOperatorType =
  | "Sole Trader"
  | "General Partnership"
  | "Limited Company"
  | "Limited Liability Partnership"
  | "Charitable Community Group"
  | "Charitable Incorporated Organisation";

export interface RegistrationDraft {
  readonly contactName: string;
  readonly contactEmail: string;
  readonly contactTelephone: string;
  readonly tradingName: string;
  readonly legalOperatorName: string;
  readonly legalOperatorType: LegalOperatorType | null;
  readonly companyRegistrationNumber: string;
  readonly tradingLocation: TradingLocation | null;
  readonly addressReference: string | null;
  readonly addressDisplayLines: readonly string[];
  readonly weeklyOpeningHours: readonly DailyOpeningHours[];
  readonly serviceIncludesHotFood: boolean | null;
  readonly alcoholService: boolean | null;
  readonly licenceDetailsRequired: boolean;
  readonly website: string;
  readonly businessDescription: string;
}

export interface RegistrationSession {
  readonly startedAt: string;
  readonly draft: RegistrationDraft;
  readonly result: RegistrationResult | null;
  readonly determinationReview: ComplianceDeterminationReview | null;
}

export interface ComplianceDeterminationReview {
  readonly determination: ComplianceDetermination;
  readonly controllingInputFingerprint: string;
}

export interface RegistrationResult {
  readonly vendorId: string;
  readonly vendorState: string;
}

export const emptyRegistrationDraft = (): RegistrationDraft => ({
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
  weeklyOpeningHours: (["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"] as const).map((day) => ({
    day,
    isClosed: false,
    isOpenAllDay: false,
    startTime: "09:00",
    endTime: "17:00",
  })),
  serviceIncludesHotFood: true,
  alcoholService: true,
  licenceDetailsRequired: false,
  website: "",
  businessDescription: "",
});
