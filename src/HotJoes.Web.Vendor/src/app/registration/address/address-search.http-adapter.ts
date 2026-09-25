import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { TradingLocation } from "../session/registration-session";
import { AddressSearchPort, AddressSearchResult } from "./address-search.port";

interface AddressSearchResponse {
  readonly results: readonly {
    readonly addressResolutionReference: string;
    readonly displayLines: readonly string[];
  }[];
}

@Injectable()
export class HttpAddressSearchAdapter implements AddressSearchPort {
  readonly #http = inject(HttpClient);

  search(
    query: string,
    tradingLocation: TradingLocation,
  ): Observable<readonly AddressSearchResult[]> {
    const params = new HttpParams()
      .set("query", query.trim())
      .set("tradingLocation", tradingLocation.toLocaleLowerCase("en-GB"));

    return this.#http
      .get<AddressSearchResponse>("/address-search", { params })
      .pipe(
        map((response) =>
          response.results.map((result) => ({
            reference: result.addressResolutionReference,
            lines: result.displayLines,
          })),
        ),
      );
  }
}
