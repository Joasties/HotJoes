# CR-013 -- HJ-106 Vendor Registration Service Contract Version 1.0 Promotion

  Metadata              Value
  --------------------- --------------------------------------------
  **Change Request**    CR-013
  **Target Document**   HJ-106 -- Vendor Registration Service Contract
  **Current Version**   0.1 Draft
  **Target Version**    1.0 Approved
  **Status**            Approved
  **Owner**             Project Architecture
  **Date**              24 July 2026

# 1. Purpose

Promote HJ-106 to the standard HotJoes document format and align its
structure with the architectural documentation set without changing the
approved business behaviour or service semantics.

# 2. Scope

Apply only the changes described in this change request.

# 3. Required Changes

## CR-013-01 -- Adopt Standard Document Structure

Restructure the document to use the standard HotJoes document format
including metadata, revision history, related documents, numbered
headings, assumptions, review checklist and standard document sections.

No business behaviour shall change.

## CR-013-02 -- Separate Business Failures from HTTP Mapping

Separate business failures from transport-specific behaviour.

Business failures shall describe business outcomes only.

HTTP status codes and transport mappings shall appear only within the
HTTP Representation section.

## CR-013-03 -- Independent Service Versioning

Introduce an explicit **Service Contract Version** independent of the
document version.

The document shall distinguish between:

-   Document Version
-   Service Contract Version

## CR-013-04 -- Clarify PATCH Ownership

Explicitly state that Vendor Managed Information is maintained by later
Vendor Management Services and is not modified through the Vendor
Registration endpoint.

The Registration endpoint remains create-only.

## CR-013-05 -- Move HTTP Examples to an Appendix

Move all request and response examples into an appendix.

The contract body shall define behaviour while appendices provide
illustrative examples only.

## CR-013-06 -- Promote to Version 1.0 Approved

Update:

-   Version: 1.0
-   Status: Approved
-   Revision History

No additional business capability shall be introduced.

# 4. Out of Scope

Do not change:

-   Business rules
-   Aggregate boundaries
-   Commands
-   Events
-   Validation
-   Address collaboration
-   Compliance ownership
-   Idempotency
-   HTTP resource model
-   Request or response schemas other than relocating examples.

# 5. Acceptance Criteria

The revised document shall:

-   Follow the standard HotJoes document structure.
-   Separate business concerns from HTTP concerns.
-   Introduce an independent Service Contract Version.
-   Clarify PATCH ownership.
-   Move HTTP examples to an appendix.
-   Be promoted to Version 1.0 Approved.
-   Preserve all existing business behaviour.

# 6. Expected Outcome

HJ-106 becomes the authoritative Version 1.0 Approved Vendor
Registration Service Contract and the template for future HotJoes Service
Contract documents.
