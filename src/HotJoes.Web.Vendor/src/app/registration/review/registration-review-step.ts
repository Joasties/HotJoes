import { HttpErrorResponse } from "@angular/common/http";
import { ChangeDetectorRef, Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { DailyOpeningHours, RegistrationDraft } from "../session/registration-session";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { VENDOR_REGISTRATION, VendorRegistrationResult } from "../submission/vendor-registration.port";
import { ComplianceDetermination, LICENCE_DETERMINATION } from "../compliance/licence-determination.port";

interface VendorApiErrorResponse {
  readonly code?: string;
  readonly message?: string;
  readonly validationErrors?: readonly { readonly field?: string; readonly message?: string }[];
}

@Component({
  selector: "app-registration-review-step",
  imports: [ReactiveFormsModule],
  templateUrl: "./registration-review-step.html",
  styleUrl: "./registration-review-step.scss",
})
export class RegistrationReviewStep {
  readonly #store = inject(RegistrationSessionStore);
  readonly #router = inject(Router);
  readonly #registration = inject(VENDOR_REGISTRATION);
  readonly #determination = inject(LICENCE_DETERMINATION);
  readonly #changeDetector = inject(ChangeDetectorRef);
  readonly declarations = inject(FormBuilder).nonNullable.group({
    authorised: [false, Validators.requiredTrue],
    accurate: [false, Validators.requiredTrue],
    termsAccepted: [false, Validators.requiredTrue],
  });
  submitting = false;
  result: VendorRegistrationResult | null = null;
  submissionError = "";

  constructor() { this.#store.start(); }

  get draft(): RegistrationDraft { return this.#store.draft()!; }
  get canSubmit(): boolean { return this.declarations.valid && this.#store.isDeterminationCurrent(); }

  hoursSummary(hours: DailyOpeningHours): string {
    if (hours.isClosed) return "Closed";
    if (hours.isOpenAllDay) return "Open all day";
    return `${hours.startTime} to ${hours.endTime}`;
  }

  yesNo(value: boolean | null): string {
    return value === null ? "Not provided" : value ? "Yes" : "No";
  }

  edit(path: "location" | "operator" | "business" | "characteristics"): void {
    void this.#router.navigate([`/registration/${path}`]);
  }

  back(): void { this.edit("characteristics"); }

  submit(): void {
    if (this.declarations.invalid || this.submitting || this.result) {
      this.declarations.markAllAsTouched();
      return;
    }
    this.submitting = true;
    this.submissionError = "";
    const reviewed = this.#store.determinationReview()!.determination;
    this.#determination.determine(this.draft).subscribe({
      next: (refreshed) => {
        if (!sameDetermination(reviewed, refreshed)) {
          this.#store.recordComplianceDetermination(refreshed);
          this.submitting = false;
          this.submissionError = "The required licence types have changed. Please review the updated determination before submitting.";
          this.#changeDetector.markForCheck();
          void this.#router.navigate(["/registration/characteristics"]);
          return;
        }
        this.#register();
      },
      error: (error: unknown) => {
        this.submissionError = this.#errorMessage(error);
        this.submitting = false;
        this.#changeDetector.markForCheck();
      },
    });
  }

  #register(): void {
    const value = this.declarations.getRawValue();
    this.#registration.register(this.draft, {
      authorisedToRegisterBusiness: value.authorised,
      informationAccurate: value.accurate,
      acceptHotJoesPlatformTerms: value.termsAccepted,
    }).subscribe({
      next: (result) => {
        this.result = result;
        this.submitting = false;
        this.#store.recordSubmission(result.vendorId, result.vendorState);
        this.#changeDetector.markForCheck();
        void this.#router.navigate(["/registration/confirmation"]);
      },
      error: (error: unknown) => {
        this.submissionError = this.#errorMessage(error);
        this.submitting = false;
        this.#changeDetector.markForCheck();
      },
    });
  }

  #errorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse || (typeof error === "object" && error !== null && "error" in error)) {
      const response = (error as { error?: VendorApiErrorResponse }).error ?? null;
      if (response?.validationErrors?.length) {
        return response.validationErrors.map((failure) => failure.message).filter(Boolean).join(" ")
          || "Some registration details need your attention.";
      }
      if (response?.message) return response.message;
      const status = (error as { status?: number }).status;
      if (status === 409) return "This registration conflicts with an existing registration.";
      if (status === 503) return "Registration is temporarily unavailable. Your details have been retained; please try again.";
    }
    return "We could not submit the registration. Your details have been retained; please try again.";
  }
}

const sameDetermination = (left: ComplianceDetermination, right: ComplianceDetermination): boolean =>
  left.ruleSetVersion === right.ruleSetVersion
  && left.items.length === right.items.length
  && left.items.every((item, index) => {
    const other = right.items[index];
    return item.requiredLicenceType === other?.requiredLicenceType
      && item.isRequired === other.isRequired;
  });
