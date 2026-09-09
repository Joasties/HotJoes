# ADR-011 – Playwright Test for Browser UI Testing and Automation

| **Document ID** | ADR-011 |
|---|---|
| **Document Title** | Playwright Test for Browser UI Testing and Automation |
| **Version** | 1.1 |
| **Status** | Accepted |
| **Classification** | Architecture |
| **Owner** | Project Architecture |
| **Last Updated** | 7 September 2026 |

---

# Revision History

| Version | Date | Description |
|---|---|---|
| 1.0 | 4 September 2026 | Applied CR-067. Recorded the approved CON-043 selection of Playwright Test with TypeScript as the browser UI testing and automation platform and established its architectural boundary. |
| 1.1 | 7 September 2026 | Applied CR-TBD. Extended the accepted Playwright decision with the approved CON-044 browser UI test architecture, isolation, synchronization, retry, diagnostic-security and CI evidence boundaries. |

---

# Related Documents

| Document ID | Title | Status |
|---|---|---|
| ADR-000 | Architectural Decision Register | Accepted |
| ADR-010 | Angular for the Vendor Web Client | Accepted |
| HJ-006 | Testing Strategy and Standards | Approved |
| HJ-007 | Enforcement Strategy | Approved |
| HJ-010 | Current Application Architectural Concerns | Approved |
| HJ-011 | Epic 1 Vendor Registration Implementation Scope | Approved |
| HJ-012 | Established Application Architecture Patterns | Approved |
| HJ-013 | Architecture and Implementation Test Catalogue | Approved |
| HJ-107 | Vendor Registration Test Catalogue | Approved |
| CON-043 | Browser UI automation platform | Approved |
| CON-044 | Browser UI test architecture and security | Approved |
| ADR-013 | Epic 1 Runtime and Deployment Composition | Accepted |

---

# 1. Context

Epic 1 requires repeatable browser-based verification and demonstration of a representative Vendor Registration journey. The journey registers a Vendor through the Web client, captures the returned Vendor ID, retrieves the persisted Vendor and verifies the displayed registered information in a controlled local or disposable non-production environment.

The automation platform must handle asynchronous browser behaviour without arbitrary timing assumptions, interact reliably with the rendered interface, isolate test execution, provide useful failure evidence, operate locally and in CI, support relevant browser engines and permit diagnostic artefacts to remain under HotJoes control.

Playwright Test, Cypress, Selenium WebDriver and WebdriverIO were compared through an independent weighted assessment. Playwright Test was assessed as the strongest fit, particularly for deterministic asynchronous interaction, semantic locators, isolated browser contexts, multi-engine execution and integrated diagnostic tracing.

# 2. Decision

Approved Browser UI Test Architecture and Security decision. HotJoes structures browser UI verification as a focused Playwright Test suite that proves representative rendered user journeys, Web-client interaction behaviour and full-stack integration without duplicating the Domain, Application, API, persistence or messaging behavioural catalogues.

The mandatory representative Epic 1 journey begins from a controlled clean environment, completes Vendor Registration through the rendered Web interface using synthetic deterministic data, observes the definitive successful API outcome, captures the returned Vendor ID, retrieves the registered Vendor through the rendered interface and verifies the displayed persisted information. Browser tests do not treat client state or an optimistic presentation as evidence of successful registration.

Browser execution uses the runtime composition approved by CON-038. Each test run or CI shard uses a uniquely identified disposable environment or equivalently isolated data boundary. Each test uses a fresh Playwright browser context and deterministic test-owned data unique within the executing environment. Tests do not depend upon execution order, previously executed tests or shared mutable Vendor identities.

Environment provisioning, migration, composition-readiness validation and final destruction are owned by the test-execution harness rather than individual page objects or journey specifications. Cleanup is idempotent. Cleanup failure is reported separately and does not replace, conceal or reverse the recorded test outcome. No production API operation is introduced solely to delete or reset test data.

The representative registration journey creates its Vendor through the rendered Web interface. A separately seeded fixed Vendor ID may be used only by a test that explicitly begins with retrieval. Test identifiers and deterministic data provide repeatability and isolation only and are not treated as authentication or authorisation.

Browser tests interact through semantic rendered behaviour. Locators prefer accessible roles and names, associated labels and requirement-significant visible text. Explicit test identifiers may be introduced only where no stable semantic locator exists and describe user-visible purpose rather than Angular structure. CSS-layout selectors, XPath, Angular component identity, dependency injection, private client state and implementation-specific DOM traversal are not used as test authority.

Ordinary synchronization uses Playwright actionability checks, retrying assertions, observable application state, approved network outcomes and the health-gated composition-readiness boundary. Fixed sleeps and arbitrary timing delays are prohibited. Timeout values are bounded, centrally configured and selected according to the operation being observed.

Browser tests do not intercept or replace the Vendor API, PostgreSQL, RabbitMQ or Compliance path in the mandatory full-stack registration-and-retrieval journey. Controlled interception may be used in separately identified Web-client tests to reproduce transport failure or otherwise impractical client states, provided the test makes no claim about the replaced server or infrastructure behaviour.

Only one registration submission is active through the UI. Playwright and the Web client do not automatically retry POST /vendors. A test of explicit user-controlled retry initiates that retry visibly and preserves server-side idempotency as the authoritative protection.

Required local execution uses no automatic test retry. CI may perform at most one explicitly reported diagnostic retry. A test that passes only on retry remains reported as flaky and does not silently satisfy a required quality gate. Repeated failure preserves evidence from the original attempt and any diagnostic retry.

Parallel execution is permitted only for tests whose browser state, synthetic data and mutable external state are isolated. Parallel tests do not weaken expected outcomes, tolerate uniqueness collisions or discover one another’s registrations. Tests that cannot satisfy those conditions execute in an explicitly serialized group.

Failure diagnostics are purposeful and access-controlled. Playwright traces and screenshots may be retained for failed execution. Video, console capture and network metadata are enabled only where they materially support diagnosis. HAR recording, unrestricted request or response body capture and indiscriminate browser-state capture are disabled by default.

Credentials, access tokens, secret values, environment-boundary credentials and uncontrolled registration information do not appear in source control, test names, reports, console output, traces, screenshots, videos or published CI artefacts. Any environment-access credential is injected ephemerally, scoped to the controlled environment and discarded with its browser context and test run.

Diagnostic artefacts are published only for failed or explicitly requested diagnostic execution, have restricted access and bounded retention, and are deleted through the governed CI artefact lifecycle. Failure to publish diagnostics does not alter the test result. Synthetic registration data is still treated as controlled diagnostic content rather than assumed harmless.

Visual-regression evidence uses reviewed baselines tied to declared viewport and interaction states. Baseline creation or replacement requires explicit review. Pixel similarity alone does not establish functional, business or accessibility conformance, and tolerance is not increased merely to suppress unexplained differences.

Browser verification includes the applicable semantic, keyboard, focus, responsive and automated-accessibility evidence required by CON-045. Representative assistive-technology and manual UX review remain separate required evidence and are not claimed as automated Playwright coverage.

The pinned Playwright Test version and its corresponding browser binaries are identical between supported local and CI execution. The required browser and viewport matrix is declared, version-controlled and executed according to the applicable quality gate. Emulation is not represented as evidence from a physical device.

This decision does not create new Vendor business rules, production authentication, user-level authorisation, public production exposure, test-only production endpoints, durable Registration Sessions or alternative service outcomes. It does not require every HJ-107 behaviour to be repeated through the browser.

Detailed directory names, helper APIs, individual timeout values, CI retention durations and the supported browser and viewport matrix may remain controlled engineering configuration provided they conform to this decision.

# 3. Rationale

- Playwright provides integrated actionability checking and automatically retrying assertions suitable for asynchronously rendered interfaces.
- It provides semantic locators capable of interacting through the user-visible and accessibility surface.
- Isolated browser contexts and parallel workers support repeatable execution without developer-profile dependence.
- Trace Viewer provides integrated action, DOM, network, console and screenshot evidence for failure diagnosis.
- Chromium, Firefox and WebKit execution is available without selecting a mandatory proprietary hosted service.
- TypeScript aligns naturally with the Angular Web-client implementation while remaining separate from Angular internals.

# 4. Alternatives Considered

## 4.1 Cypress

Cypress is a credible alternative with a strong interactive local debugging experience and retryable commands. It was not selected because Playwright provided the stronger combined fit for isolation, integrated trace evidence, multi-engine support and optional-service independence.

## 4.2 Selenium WebDriver

Selenium provides a mature standards-based ecosystem and broad language and browser support. It was not selected because achieving the required deterministic waiting and integrated diagnostic experience would require more supporting framework engineering.

## 4.3 WebdriverIO

WebdriverIO is a credible JavaScript and TypeScript WebDriver framework with automatic interaction waiting and extensible reporting. It was not selected because it did not establish an advantage over Playwright on the highest-weighted determinism and failure-diagnosis criteria.

# 5. Consequences

- Browser coverage remains focused on rendered journeys and Web-client interaction rather than duplicating HJ-107.
- The execution harness owns environment lifecycle and the mandatory journey uses the real Epic 1 path.
- Semantic locator, deterministic waiting, retry, parallelisation and diagnostic policies become enforceable architecture.
- Diagnostic evidence receives explicit access, retention and leakage controls.
- Retry-only success remains visible as flakiness and cannot silently satisfy a required gate.
- Detailed helper APIs, timeout values, retention durations and browser/viewport matrices remain controlled engineering configuration.

# 6. Verification

Conformance requires the following evidence:

Verify the mandatory full-stack rendered journey from clean environment provisioning through Vendor Registration, definitive successful response, returned Vendor ID capture, registered-Vendor retrieval, displayed persisted-information comparison and environment destruction.

Verify harness ownership of provisioning, migration, composition readiness and cleanup; unique run or shard identity; fresh browser context per test; deterministic unique synthetic data; execution-order independence; idempotent cleanup; separate cleanup-failure reporting; and absence of test-only production reset or deletion endpoints.

Verify semantic locator precedence using accessible roles and names, labels and requirement-significant visible text. Reject CSS-layout selectors, XPath, Angular component identity, dependency injection, private state and implementation-specific DOM traversal. Review every explicit test identifier for genuine semantic necessity and user-visible purpose.

Verify Playwright actionability, retrying assertions, observable UI state, approved network outcomes and health-gated readiness as the synchronization mechanisms. Reject fixed sleeps, arbitrary delays and unbounded or locally invented timeouts.

Verify that the mandatory full-stack journey uses the real Vendor API, PostgreSQL, RabbitMQ and Compliance path. Ensure controlled network interception is confined to separately identified Web-client tests and does not claim verification of replaced components.

Verify one active registration submission, absence of automatic POST /vendors retry and explicit user-controlled retry behaviour. Run required tests without automatic local retry. Verify that CI permits no more than one diagnostic retry, reports retry-only success as flaky and prevents it from silently satisfying the required gate.

Run repeated, reordered and parallel tests to prove browser-state, data and mutable-environment isolation. Verify that collisions, shared Vendor discovery and weakened expected outcomes are not tolerated and that non-isolatable tests are explicitly serialized.

Deliberately trigger application, assertion, network, automation and cleanup failures. Verify useful evidence from the original attempt and any permitted diagnostic retry. Verify that traces and screenshots are limited to failed or explicitly requested diagnostics; video, console and network metadata are purposeful; HAR and unrestricted body capture are disabled by default; access is restricted; retention is bounded; and publication failure does not alter the test result.

Scan source, configuration, browser storage, reports, console output and published artefacts for credentials, tokens, secret values, environment-access credentials and uncontrolled registration information. Verify ephemeral injection, environment scoping and disposal of any controlled access credential.

Verify reviewed visual baselines for declared viewports and interaction states, explicit baseline-change approval and rejection of unexplained tolerance increases. Combine Playwright journeys with semantic, keyboard, focus, responsive and automated-accessibility checks while retaining representative assistive-technology and manual UX review as separate evidence.

Verify identical pinned Playwright and browser-binary versions locally and in CI, a declared version-controlled browser and viewport matrix, execution through the applicable quality gate and no claim that browser emulation constitutes physical-device evidence.
