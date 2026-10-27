import { expect, test } from "@playwright/test";
import {
  createBrowserTestIdentity,
  createBrowserTestIdentityRegistry,
} from "../support/browser-runtime-lifecycle.mjs";

test.describe.configure({ mode: "parallel" });

test("AI-BROWSER-007 parallel workers receive distinct deterministic identities", () => {
  const first = identity({ workerIndex: 0, titlePath: ["parallel", "first"] });
  const second = identity({ workerIndex: 1, titlePath: ["parallel", "second"] });
  const reorderedSecond = identity({ workerIndex: 1, titlePath: ["parallel", "second"] });
  const reorderedFirst = identity({ workerIndex: 0, titlePath: ["parallel", "first"] });

  expect(first).toBe(reorderedFirst);
  expect(second).toBe(reorderedSecond);
  expect(first).not.toBe(second);
});

test("AI-BROWSER-007 collision injection is rejected rather than weakening assertions", () => {
  const registry = createBrowserTestIdentityRegistry();
  const reservedIdentity = identity({
    workerIndex: 0,
    titlePath: ["parallel", "collision"],
  });

  registry.reserve(reservedIdentity);

  expect(() => registry.reserve(reservedIdentity))
    .toThrow("Browser test identity collision detected.");
});

test("AI-BROWSER-007 retry and repeat executions cannot reuse mutable test identity", () => {
  const initial = identity({
    workerIndex: 0,
    titlePath: ["parallel", "repeat and retry"],
  });
  const repeated = identity({
    workerIndex: 0,
    repeatEachIndex: 1,
    titlePath: ["parallel", "repeat and retry"],
  });
  const retried = identity({
    workerIndex: 0,
    retryIndex: 1,
    titlePath: ["parallel", "repeat and retry"],
  });

  expect(new Set([initial, repeated, retried]).size).toBe(3);
});

function identity({
  workerIndex,
  titlePath,
  repeatEachIndex = 0,
  retryIndex = 0,
}: {
  workerIndex: number;
  titlePath: string[];
  repeatEachIndex?: number;
  retryIndex?: number;
}): string {
  return createBrowserTestIdentity({
    runId: "parallel-isolation-run",
    titlePath,
    workerIndex,
    repeatEachIndex,
    retryIndex,
  });
}
