import { HttpErrorResponse } from "@angular/common/http";
import { ChangeDetectorRef, Component, inject, OnDestroy } from "@angular/core";
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { DailyOpeningHours, TradingDay } from "../session/registration-session";
import { RegistrationSessionStore } from "../session/registration-session.store";
import {
  ComplianceDetermination,
  LICENCE_DETERMINATION,
  RequiredLicenceType,
} from "../compliance/licence-determination.port";

const days: readonly TradingDay[] = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
const explicitChoice = (control: AbstractControl<boolean | null>): ValidationErrors | null => control.value === null ? { required: true } : null;
const validDailyHours = (control: AbstractControl): ValidationErrors | null => {
  const value = control.value as DailyOpeningHours;
  if (value.isClosed || value.isOpenAllDay) return null;
  if (!value.startTime || !value.endTime) return { timesRequired: true };
  return value.startTime === value.endTime ? { useOpenAllDay: true } : null;
};

@Component({
  selector: "app-trading-characteristics-step",
  imports: [ReactiveFormsModule],
  templateUrl: "./trading-characteristics-step.html",
  styleUrl: "./trading-characteristics-step.scss",
})
export class TradingCharacteristicsStep implements OnDestroy {
  readonly #store = inject(RegistrationSessionStore);
  readonly #router = inject(Router);
  readonly #builder = inject(FormBuilder);
  readonly #determination = inject(LICENCE_DETERMINATION);
  readonly #changeDetector = inject(ChangeDetectorRef);
  readonly days = days;
  customHours = false;
  #hoursBeforeEditing: DailyOpeningHours[] = [];
  #activeDetermination: Subscription | null = null;
  determinationResult: ComplianceDetermination | null = null;
  determinationError = "";
  determining = false;
  readonly form = this.#builder.group({
    everydayStart: ["09:00", Validators.required],
    everydayEnd: ["17:00", Validators.required],
    weeklyOpeningHours: this.#builder.array(days.map((day) => this.#builder.nonNullable.group({
      day: [day],
      isClosed: [false],
      isOpenAllDay: [false],
      startTime: ["09:00" as string | null],
      endTime: ["17:00" as string | null],
    }, { validators: validDailyHours }))),
    serviceIncludesHotFood: [true as boolean | null, explicitChoice],
    alcoholService: [true as boolean | null, explicitChoice],
    licenceDetailsRequired: this.#builder.nonNullable.control(false),
    website: ["", Validators.pattern(/^$|^https:\/\/[^\s]+$/i)],
    businessDescription: ["", Validators.maxLength(2000)],
  });

  constructor() {
    this.#store.start();
    const draft = this.#store.draft();
    if (!draft) return;
    draft.weeklyOpeningHours.forEach((hours, index) => this.form.controls.weeklyOpeningHours.at(index)?.patchValue(hours));
    this.customHours = !this.#isEverydaySchedule(draft.weeklyOpeningHours);
    this.form.patchValue({
      everydayStart: draft.weeklyOpeningHours[0]?.startTime ?? "09:00",
      everydayEnd: draft.weeklyOpeningHours[0]?.endTime ?? "17:00",
      serviceIncludesHotFood: draft.serviceIncludesHotFood ?? true,
      alcoholService: draft.alcoholService ?? true,
      licenceDetailsRequired: draft.licenceDetailsRequired,
      website: draft.website,
      businessDescription: draft.businessDescription,
    });
    if (this.#store.isDeterminationCurrent()) {
      this.determinationResult = this.#store.determinationReview()?.determination ?? null;
    } else {
      this.#refreshDetermination();
    }
  }

  choose(control: "serviceIncludesHotFood" | "alcoholService", value: boolean): void {
    this.form.controls[control].setValue(value);
    this.#refreshDetermination();
  }
  applyEveryday(): void {
    const startTime = this.form.controls.everydayStart.value ?? "";
    const endTime = this.form.controls.everydayEnd.value ?? "";
    this.form.controls.weeklyOpeningHours.controls.forEach((control) => control.patchValue({ isClosed: false, isOpenAllDay: false, startTime, endTime }));
    this.#refreshDetermination();
  }
  toggleDayState(index: number, state: "openAllDay" | "closed", selected: boolean): void {
    const control = this.form.controls.weeklyOpeningHours.at(index);
    const timed = !selected;
    control.patchValue({
      isClosed: state === "closed" && selected,
      isOpenAllDay: state === "openAllDay" && selected,
      startTime: timed ? control.value.startTime ?? "09:00" : null,
      endTime: timed ? control.value.endTime ?? "17:00" : null,
    });
    control.updateValueAndValidity();
  }
  openCustomHours(dialog: HTMLDialogElement): void {
    this.#hoursBeforeEditing = this.form.controls.weeklyOpeningHours.getRawValue().map((day) => ({ ...day })) as DailyOpeningHours[];
    dialog.showModal();
  }
  cancelCustomHours(dialog: HTMLDialogElement): void {
    this.#hoursBeforeEditing.forEach((day, index) => this.form.controls.weeklyOpeningHours.at(index).setValue({ ...day }));
    dialog.close();
  }
  saveCustomHours(dialog: HTMLDialogElement): void {
    const hours = this.form.controls.weeklyOpeningHours;
    if (hours.invalid) { hours.markAllAsTouched(); return; }
    this.customHours = true;
    dialog.close();
    this.#refreshDetermination();
  }
  hoursSummary(index: number): string {
    const value = this.form.controls.weeklyOpeningHours.at(index).getRawValue();
    if (value.isClosed) return "Closed";
    if (value.isOpenAllDay) return "Open all day";
    return `${value.startTime} to ${value.endTime}`;
  }
  open(dialog: HTMLDialogElement): void { dialog.showModal(); }
  close(dialog: HTMLDialogElement): void { dialog.close(); }
  saveLicenceDetails(dialog: HTMLDialogElement): void {
    this.#persistTradingCharacteristics();
    dialog.close();
  }
  saveWebsite(dialog: HTMLDialogElement): void {
    const website = this.form.controls.website;
    website.markAsTouched();
    if (website.invalid) return;
    this.#persistTradingCharacteristics();
    dialog.close();
  }
  saveBusinessDescription(dialog: HTMLDialogElement): void {
    const description = this.form.controls.businessDescription;
    description.markAsTouched();
    if (description.invalid) return;
    this.#persistTradingCharacteristics();
    dialog.close();
  }
  showError(control: "website" | "businessDescription"): boolean { const item = this.form.controls[control]; return item.invalid && item.touched; }
  ngOnDestroy(): void {
    this.#activeDetermination?.unsubscribe();
  }
  #refreshDetermination(): void {
    if (this.form.invalid) return;
    this.#persistTradingCharacteristics();
    this.determining = true;
    this.determinationError = "";
    this.determinationResult = null;
    this.#activeDetermination?.unsubscribe();
    this.#activeDetermination = this.#determination.determine(this.#store.draft()!).subscribe({
      next: (determination) => {
        this.determining = false;
        try {
          this.#store.recordComplianceDetermination(determination);
          this.determinationResult = determination;
        } catch (error: unknown) {
          this.determinationError = this.#clientProcessingErrorMessage(error);
        }
        this.#changeDetector.markForCheck();
      },
      error: (error: unknown) => {
        this.determinationError = this.#determinationErrorMessage(error);
        this.determining = false;
        this.#changeDetector.markForCheck();
      },
    });
  }
  #persistTradingCharacteristics(): void {
    const value = this.form.getRawValue();
    this.#store.updateTradingCharacteristics({
      weeklyOpeningHours: value.weeklyOpeningHours as DailyOpeningHours[],
      serviceIncludesHotFood: value.serviceIncludesHotFood ?? true,
      alcoholService: value.alcoholService ?? true,
      licenceDetailsRequired: value.licenceDetailsRequired,
      website: value.website?.trim() ?? "",
      businessDescription: value.businessDescription?.trim() ?? "",
    });
  }
  confirmDeterminationAndContinue(): void {
    if (!this.determinationResult || !this.#store.isDeterminationCurrent()) return;
    void this.#router.navigate(["/registration/review"]);
  }
  licenceName(type: RequiredLicenceType): string {
    return licenceNames[type];
  }
  back(): void { void this.#router.navigate(["/registration/business"]); }
  #isEverydaySchedule(hours: readonly DailyOpeningHours[]): boolean {
    const first = hours[0];
    return Boolean(first && !first.isClosed && !first.isOpenAllDay && hours.every((day) => !day.isClosed && !day.isOpenAllDay && day.startTime === first.startTime && day.endTime === first.endTime));
  }
  #determinationErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse || (typeof error === "object" && error !== null && "error" in error)) {
      const response = (error as { error?: { message?: string } }).error;
      if (response?.message) return response.message;
    }
    return "We could not determine the required licence types. Your details have been retained; please try again.";
  }
  #clientProcessingErrorMessage(error: unknown): string {
    const detail = error instanceof Error && error.message ? ` ${error.message}` : "";
    return `The required licence types were returned but could not be prepared for review.${detail}`;
  }
}

const licenceNames: Readonly<Record<RequiredLicenceType, string>> = {
  foodBusinessRegistration: "Food Business Registration",
  streetTradingLicence: "Street Trading Licence",
  lateNightRefreshmentLicence: "Late Night Refreshment Licence",
  premisesLicence: "Premises Licence",
  personalLicenceHolder: "Personal Licence Holder",
};
