import { Injectable, computed, signal } from "@angular/core";
import {
  emptyRegistrationDraft,
  LegalOperatorType,
  RegistrationDraft,
  RegistrationSession,
  TradingLocation,
} from "./registration-session";
import { ComplianceDetermination } from "../compliance/licence-determination.port";

@Injectable()
export class RegistrationSessionStore {
  readonly #session = signal<RegistrationSession | null>(null);

  readonly session = this.#session.asReadonly();
  readonly isActive = computed(() => this.#session() !== null);
  readonly draft = computed(() => this.#session()?.draft ?? null);
  readonly determinationReview = computed(() => this.#session()?.determinationReview ?? null);

  start(): void {
    if (this.#session()) return;

    this.#session.set({
      startedAt: new Date().toISOString(),
      draft: emptyRegistrationDraft(),
      result: null,
      determinationReview: null,
    });
  }

  updateBusinessIdentity(
    change: Pick<
      RegistrationDraft,
      | "tradingName"
      | "legalOperatorName"
      | "legalOperatorType"
      | "companyRegistrationNumber"
    >,
  ): void {
    this.#updateDraft((draft) => ({ ...draft, ...change }));
  }

  updatePrimaryContact(
    change: Pick<RegistrationDraft, "contactName" | "contactEmail" | "contactTelephone">,
  ): void {
    this.#updateDraft((draft) => ({ ...draft, ...change }));
  }

  updateTradingCharacteristics(
    change: Pick<
      RegistrationDraft,
      | "weeklyOpeningHours"
      | "serviceIncludesHotFood"
      | "alcoholService"
      | "licenceDetailsRequired"
      | "website"
      | "businessDescription"
    >,
  ): void {
    this.#updateDraft((draft) => ({ ...draft, ...change }));
  }

  selectAddress(
    addressReference: string,
    addressDisplayLines: readonly string[],
  ): void {
    this.#updateDraft((draft) => ({
      ...draft,
      addressReference,
      addressDisplayLines: [...addressDisplayLines],
    }));
  }

  clearAddress(): void {
    this.#updateDraft((draft) => ({
      ...draft,
      addressReference: null,
      addressDisplayLines: [],
    }));
  }

  selectLegalOperatorType(legalOperatorType: LegalOperatorType): void {
    this.#updateDraft((draft) => ({
      ...draft,
      legalOperatorType,
      companyRegistrationNumber:
        draft.legalOperatorType === legalOperatorType
          ? draft.companyRegistrationNumber
          : "",
    }));
  }

  changeTradingLocation(
    next: TradingLocation,
    destructiveChangeConfirmed: boolean,
  ): boolean {
    const current = this.#requireSession();
    const wouldDiscardAddress = current.draft.addressReference !== null;

    if (wouldDiscardAddress && !destructiveChangeConfirmed) return false;

    const draft = {
      ...current.draft,
      tradingLocation: next,
      addressReference: null,
      addressDisplayLines: [],
    };
    const determinationReview = current.determinationReview
      && current.determinationReview.controllingInputFingerprint === controllingInputFingerprint(draft)
      ? current.determinationReview
      : null;
    this.#session.set({ ...current, draft, determinationReview });
    return true;
  }

  reset(confirmed: boolean): boolean {
    if (!confirmed || !this.#session()) return false;

    this.#session.update(
      (current) =>
        current && {
          ...current,
          draft: emptyRegistrationDraft(),
          determinationReview: null,
        },
    );
    return true;
  }

  abandon(): void {
    this.#session.set(null);
  }

  recordSubmission(vendorId: string, vendorState: string): void {
    const current = this.#requireSession();
    this.#session.set({ ...current, result: { vendorId, vendorState } });
  }

  recordComplianceDetermination(determination: ComplianceDetermination): void {
    const current = this.#requireSession();
    this.#session.set({
      ...current,
      determinationReview: {
        determination,
        controllingInputFingerprint: controllingInputFingerprint(current.draft),
      },
    });
  }

  isDeterminationCurrent(): boolean {
    const current = this.#session();
    return Boolean(current?.determinationReview
      && current.determinationReview.controllingInputFingerprint === controllingInputFingerprint(current.draft));
  }

  #updateDraft(update: (draft: RegistrationDraft) => RegistrationDraft): void {
    const current = this.#requireSession();
    const draft = update(current.draft);
    const determinationReview = current.determinationReview
      && current.determinationReview.controllingInputFingerprint === controllingInputFingerprint(draft)
      ? current.determinationReview
      : null;
    this.#session.set({ ...current, draft, determinationReview });
  }

  #requireSession(): RegistrationSession {
    const current = this.#session();
    if (!current)
      throw new Error("A registration session has not been started.");
    return current;
  }
}

const controllingInputFingerprint = (draft: RegistrationDraft): string => JSON.stringify({
  legalOperatorType: draft.legalOperatorType,
  tradingLocation: draft.tradingLocation,
  weeklyOpeningHours: draft.weeklyOpeningHours.map((hours) => ({
    day: hours.day,
    isClosed: hours.isClosed,
    isOpenAllDay: hours.isOpenAllDay,
    startTime: hours.startTime,
    endTime: hours.endTime,
  })),
  serviceIncludesHotFood: draft.serviceIncludesHotFood,
  alcoholService: draft.alcoholService,
  addressReference: draft.addressReference,
});
