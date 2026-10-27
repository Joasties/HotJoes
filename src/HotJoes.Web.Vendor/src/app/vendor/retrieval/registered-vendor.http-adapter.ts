import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import {
  RegisteredVendorDetails,
  RegisteredVendorRetrievalPort,
} from "./registered-vendor.port";

@Injectable()
export class HttpRegisteredVendorAdapter implements RegisteredVendorRetrievalPort {
  private readonly http = inject(HttpClient);

  retrieve(vendorId: string): Observable<RegisteredVendorDetails> {
    return this.http.get<RegisteredVendorDetails>(
      `/vendors/${encodeURIComponent(vendorId)}`,
    );
  }
}
