# CR-072 - Expose the Address-Owned Registration Search Boundary

| Property | Value |
|---|---|
| Change Request | CR-072 |
| Status | Approved |
| Date | 10 September 2026 |
| Owner | Project Architecture |
| Affected concerns | CON-009, CON-030, CON-034, CON-038, CON-042, CON-045 |

## 1. Reason

The approved baseline assigns address search, validation, normalisation and canonical identity to the Address bounded context. It requires the Web client to select a complete authoritative Address result and retain only its permanent opaque Address Resolution reference plus permitted display information.

The current implementation instead duplicates the synthetic catalogue inside the Angular client. That makes the browser, rather than the Address capability, the source of search results and permits the Web copy to drift from the catalogue used during authoritative registration-time resolution.

HJ-011 §2.2 and ADR-006 already require Address-owned search and selection. However, HJ-011 §2.3, AI-API-005 and AI-GW-001 currently prohibit every public Address search route. The baseline therefore prevents its own required browser journey from crossing the approved Address boundary.

## 2. Decision Requested

Approve one narrow, read-only Address-search adapter for the Epic 1 registration journey:

`GET /address-search?query={query}&tradingLocation={restaurant|stall|kitchen}`

The endpoint is hosted by the existing Vendor API deployable as a thin composition adapter and forwarded unchanged by the YARP Edge. Hosting does not transfer business ownership: search behaviour and results remain owned by the Address Application boundary.

The endpoint:

- accepts a trimmed, non-empty query of at most 200 characters and one lower-camel-case Trading Location;
- returns `400 Bad Request` for malformed query parameters;
- returns `200 OK` with `{ "results": [] }` when no result matches;
- returns each match as only `addressResolutionReference` and ordered non-blank `displayLines`;
- returns no Canonical Address Identifier, regulatory authority, internal catalogue revision or complete resolution representation;
- is read-only and neither creates nor consumes an Address Resolution reference;
- searches the same version-controlled singleton stub catalogue used by authoritative registration-time resolution;
- restricts results to the declared Trading Location; and
- exposes no general Address, catalogue-management or Address-resolution route.

The Angular client shall remove its duplicated catalogue. A typed HTTP adapter behind the existing `AddressSearchPort` invokes the same-origin route. The Registration Session may retain only the selected opaque reference and permitted display lines. Search transport failure is presented as retryable interaction failure and does not fabricate a result.

## 3. Controlled Baseline Changes

### HJ-011

- Add the thin `GET /address-search` interaction adapter to §2.3.
- Clarify that the prohibition on collection/search endpoints applies to Vendor resources, not the approved Address-search adapter.
- Expand the Edge allowlist from the two Vendor operations to those operations plus Address search.
- In the synthetic bootstrap section, replace the Web-owned catalogue copy with one server-side version-controlled catalogue searched and resolved by the same Address stub instance.

### HJ-013

- Revise AI-API-005 from “no public Address API” to “only the approved non-authoritative Address-search projection is exposed”; continue to prohibit catalogue, resolution and general Address routes.
- Revise AI-GW-001 and AI-RUNTIME-004 to include the approved `GET /address-search` route.
- Revise AI-RUNTIME-007 so one server-side catalogue instance supplies both search and authoritative resolution; remove the requirement for a separate Web catalogue revision.
- Retain AI-WEB-006 for typed HTTP-adapter confinement and revise AI-WEB-007 to require server-returned selectable results, permitted session state and absence of Address-owned authority values.
- Add behavioural evidence for query validation, Trading Location filtering, stable reference selection, empty results and safe transport failure without duplicating registration-resolution tests.

### HJ-000

- Record the approved revisions of HJ-011 and HJ-013 and add this Change Request to the applied baseline evidence.

HJ-106 remains the authoritative Vendor Registration and retrieval contract. This change does not add a Vendor search operation or alter `POST /vendors` or `GET /vendors/{vendorId}`. The supporting Address-search wire contract is governed by the revised HJ-011 scope and ADR-006 ownership decision.

## 4. Implementation Consequences

- `StubAddressApplication` implements a separate `IAddressSearchService` alongside `IAddressResolutionService`.
- Dependency injection exposes both interfaces over the same singleton stub.
- The API maps the narrow search response and the Edge adds one method-and-path-specific allowlist entry.
- Angular `HttpClient` remains confined to an HTTP adapter; the component and Registration Session depend only on typed client representations.
- Existing negative tests continue to prove `/addresses`, `/address-resolution` and `/address-bootstrap-catalogue` are absent.

## 5. Verification

Acceptance requires:

1. Address Application tests proving case-insensitive query matching, Trading Location isolation and stable resolvable references.
2. API tests proving exact query validation and response-member exclusion.
3. OpenAPI evidence containing only the two Vendor operations and the approved Address-search operation.
4. Edge tests proving only the exact `GET /address-search` route is added and forwarded without semantic transformation.
5. Angular adapter tests proving query mapping and response projection.
6. Journey tests proving explicit selection, session retention and safe failure presentation.
7. The complete .NET and Angular suites remain green.

## 6. Explicit Non-Goals

- No production address-provider integration.
- No general Address API, direct catalogue download or public resolution endpoint.
- No client authority over canonical Address data or regulatory authorities.
- No change to Vendor identity, registration semantics, persistence or events.
- No browser-owned synthetic fallback when the Address boundary is unavailable.
