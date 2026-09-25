import { ChangeDetectorRef, Component, inject, OnDestroy } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { ADDRESS_SEARCH, AddressSearchResult } from "../address/address-search.port";
import { LegalOperatorType } from "../session/registration-session";
import { RegistrationSessionStore } from "../session/registration-session.store";

const companyNumberTypes: readonly LegalOperatorType[] = ["Limited Company", "Limited Liability Partnership", "Charitable Incorporated Organisation"];

@Component({
  selector: "app-business-identity-step",
  imports: [ReactiveFormsModule],
  templateUrl: "./business-identity-step.html",
  styleUrl: "./business-identity-step.scss",
})
export class BusinessIdentityStep implements OnDestroy {
  readonly #store = inject(RegistrationSessionStore);
  readonly #router = inject(Router);
  readonly #addresses = inject(ADDRESS_SEARCH);
  readonly #changeDetector = inject(ChangeDetectorRef);
  readonly form = inject(FormBuilder).nonNullable.group({
    contactName: ["", [Validators.required, Validators.maxLength(100)]],
    contactTelephone: ["", [Validators.required, Validators.pattern(/^(?:\+44|0)[0-9 ()-]{9,16}$/)]],
    contactEmail: ["", [Validators.required, Validators.email, Validators.pattern(/^[\x00-\x7F]+$/)]],
    tradingName: ["", [Validators.required, Validators.maxLength(160)]],
    legalOperatorName: ["", [Validators.required, Validators.maxLength(160)]],
    companyRegistrationNumber: ["", [Validators.maxLength(8), Validators.pattern(/^(?:[A-Za-z]{2})?\d{6,8}$/)]],
    addressQuery: [""],
  });
  addressResults: readonly AddressSearchResult[] = [];
  selectedAddress: AddressSearchResult | null = null;
  addressTouched = false;
  addressSearchFailed = false;
  #activeSearch?: Subscription;

  constructor() {
    this.#store.start();
    const draft = this.#store.draft();
    if (draft) {
      this.form.patchValue({
        contactName: draft.contactName,
        contactTelephone: draft.contactTelephone,
        contactEmail: draft.contactEmail,
        tradingName: draft.tradingName,
        legalOperatorName: draft.legalOperatorName,
        companyRegistrationNumber: draft.companyRegistrationNumber,
      });
      this.selectedAddress = draft.addressReference
        ? { reference: draft.addressReference, lines: draft.addressDisplayLines }
        : null;
    }
    this.#configureCompanyNumber();
  }

  get legalOperatorType(): LegalOperatorType | null { return this.#store.draft()?.legalOperatorType ?? null; }
  get requiresCompanyNumber(): boolean { return this.legalOperatorType !== null && companyNumberTypes.includes(this.legalOperatorType); }
  showError(name: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[name];
    return control.invalid && (control.touched || this.form.touched);
  }
  searchAddress(): void {
    const location = this.#store.draft()?.tradingLocation;
    const query = this.form.controls.addressQuery.value.trim();
    this.addressSearchFailed = false;
    this.addressResults = [];
    if (!location || !query) return;

    this.#activeSearch?.unsubscribe();
    this.#activeSearch = this.#addresses.search(query, location).subscribe({
      next: (results) => {
        this.addressResults = results;
        this.#changeDetector.markForCheck();
      },
      error: () => {
        this.addressSearchFailed = true;
        this.#changeDetector.markForCheck();
      },
    });
  }
  selectAddress(address: AddressSearchResult): void {
    this.selectedAddress = address;
    this.addressResults = [];
    this.form.controls.addressQuery.setValue(address.lines.join(", "));
    this.#store.selectAddress(address.reference, address.lines);
  }
  clearAddress(): void {
    this.selectedAddress = null;
    this.form.controls.addressQuery.setValue("");
    this.addressResults = [];
    this.addressSearchFailed = false;
    this.#store.clearAddress();
  }
  continue(): void {
    this.addressTouched = true;
    if (this.form.invalid || !this.selectedAddress) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();
    const legalOperatorType = this.legalOperatorType;
    if (!legalOperatorType) { void this.#router.navigate(["/registration/operator"]); return; }
    this.#store.updatePrimaryContact({ contactName: value.contactName, contactTelephone: value.contactTelephone, contactEmail: value.contactEmail });
    this.#store.updateBusinessIdentity({ tradingName: value.tradingName, legalOperatorName: value.legalOperatorName, legalOperatorType, companyRegistrationNumber: value.companyRegistrationNumber.toUpperCase() });
    void this.#router.navigate(["/registration/characteristics"]);
  }
  back(): void { void this.#router.navigate(["/registration/operator"]); }
  ngOnDestroy(): void { this.#activeSearch?.unsubscribe(); }
  #configureCompanyNumber(): void {
    const control = this.form.controls.companyRegistrationNumber;
    control.setValidators([...(this.requiresCompanyNumber ? [Validators.required] : []), Validators.maxLength(8), Validators.pattern(/^(?:[A-Za-z]{2})?\d{6,8}$/)]);
    if (!this.requiresCompanyNumber) control.setValue("", { emitEvent: false });
    control.updateValueAndValidity({ emitEvent: false });
  }
}
