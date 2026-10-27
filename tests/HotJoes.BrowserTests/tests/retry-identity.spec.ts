import { expect, test } from "@playwright/test";
import { createBrowserTestIdentity } from "../support/browser-runtime-lifecycle.mjs";

test("AI-BROWSER-006 diagnostic retry receives distinct deterministic test data", () => {
  const attempt = {
    runId: "retry-policy-run",
    titlePath: ["registration", "authoritative submission"],
    workerIndex: 0,
    repeatEachIndex: 0,
  };

  const original = createBrowserTestIdentity({
    ...attempt,
    retryIndex: 0,
  });
  const diagnosticRetry = createBrowserTestIdentity({
    ...attempt,
    retryIndex: 1,
  });

  expect(createBrowserTestIdentity({
    ...attempt,
    retryIndex: 0,
  })).toBe(original);
  expect(diagnosticRetry).not.toBe(original);
});
