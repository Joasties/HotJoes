import { ChangeDetectorRef, Component, OnInit, inject } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import {
  REGISTERED_VENDOR_RETRIEVAL,
  RegisteredVendorDailyOpeningHours,
  RegisteredVendorDetails,
} from "./registered-vendor.port";

@Component({
  selector: "app-registered-vendor-details-step",
  standalone: true,
  templateUrl: "./registered-vendor-details-step.html",
  styleUrl: "./registered-vendor-details-step.scss",
})
export class RegisteredVendorDetailsStep implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly retrieval = inject(REGISTERED_VENDOR_RETRIEVAL);
  private readonly changeDetector = inject(ChangeDetectorRef);

  loading = true;
  details: RegisteredVendorDetails | null = null;
  retrievalError = "";

  ngOnInit(): void {
    const vendorId = this.route.snapshot.paramMap.get("vendorId");
    if (!vendorId) {
      this.failRetrieval();
      return;
    }

    this.retrieval.retrieve(vendorId).subscribe({
      next: (details) => {
        this.details = details;
        this.loading = false;
        this.changeDetector.markForCheck();
      },
      error: () => this.failRetrieval(),
    });
  }

  dayName(day: string): string {
    return day.length === 0 ? day : `${day[0].toUpperCase()}${day.slice(1)}`;
  }

  openingHours(day: RegisteredVendorDailyOpeningHours): string {
    if (day.isClosed) return "Closed";
    if (day.isOpenAllDay) return "Open all day";
    return `${day.startTime?.slice(0, 5)} to ${day.endTime?.slice(0, 5)}`;
  }

  yesNo(value: boolean): string {
    return value ? "Yes" : "No";
  }

  private failRetrieval(): void {
    this.loading = false;
    this.retrievalError =
      "Registration succeeded, but the registered details could not be retrieved. Please try again safely.";
    this.changeDetector.markForCheck();
  }
}
