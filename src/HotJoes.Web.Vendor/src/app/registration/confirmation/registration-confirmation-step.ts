import { HttpErrorResponse } from "@angular/common/http";
import { ChangeDetectorRef, Component, inject } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { toSignal } from "@angular/core/rxjs-interop";
import {
  COMMUNITY_PARTICIPATION,
  ContactPreference,
} from "../community/community-participation.port";
import { RegistrationSessionStore } from "../session/registration-session.store";

@Component({
  selector: "app-registration-confirmation-step",
  imports: [ReactiveFormsModule],
  templateUrl: "./registration-confirmation-step.html",
  styleUrl: "./registration-confirmation-step.scss",
})
export class RegistrationConfirmationStep {
  readonly #store = inject(RegistrationSessionStore);
  readonly #router = inject(Router);
  readonly #community = inject(COMMUNITY_PARTICIPATION);
  readonly #changeDetector = inject(ChangeDetectorRef);

  readonly form = inject(FormBuilder).nonNullable.group({
    contactPreference: ["email" as ContactPreference],
    keepInvolved: [false],
  });
  readonly #keepInvolved = toSignal(
    this.form.controls.keepInvolved.valueChanges,
    { initialValue: this.form.controls.keepInvolved.value },
  );

  submitting = false;
  submissionError = "";

  get tradingName(): string {
    return this.#store.draft()?.tradingName || "Vendor";
  }

  get canJoin(): boolean {
    return this.#keepInvolved() && !this.submitting;
  }

  notNow(): void {
    if (this.submitting) return;
    void this.#router.navigate([
      "/registration/community-not-joined",
    ]);
  }

  joinCommunity(): void {
    if (!this.canJoin) return;

    const result = this.#store.session()?.result;
    if (!result) {
      this.submissionError =
        "Your completed registration could not be identified. " +
        "Please return to the registration journey.";
      return;
    }

    this.submitting = true;
    this.submissionError = "";
    this.#community.join(
      result.vendorId,
      this.form.controls.contactPreference.value,
    ).subscribe({
      next: () => {
        this.submitting = false;
        this.#changeDetector.markForCheck();
        void this.#router.navigate([
          "/registration/community-joined",
        ]);
      },
      error: (error: unknown) => {
        this.submitting = false;
        this.submissionError = this.#errorMessage(error);
        this.#changeDetector.markForCheck();
      },
    });
  }

  #errorMessage(error: unknown): string {
    const status = error instanceof HttpErrorResponse
      ? error.status
      : typeof error === "object" && error !== null && "status" in error
        ? (error as { readonly status?: number }).status
        : undefined;

    if (status === 409) {
      return "This Vendor has already joined with a different contact preference.";
    }

    if (status === 400 || status === 404) {
      return "We could not record Community participation for this registration.";
    }

    return "Community participation is temporarily unavailable. " +
      "Your selections have been retained; please try again.";
  }
}
