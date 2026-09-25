import { InjectionToken } from "@angular/core";
import { Observable } from "rxjs";
import { TradingLocation } from "../session/registration-session";

export interface AddressSearchResult {
  readonly reference: string;
  readonly lines: readonly string[];
}

export interface AddressSearchPort {
  search(
    query: string,
    tradingLocation: TradingLocation,
  ): Observable<readonly AddressSearchResult[]>;
}

export const ADDRESS_SEARCH = new InjectionToken<AddressSearchPort>("ADDRESS_SEARCH");
