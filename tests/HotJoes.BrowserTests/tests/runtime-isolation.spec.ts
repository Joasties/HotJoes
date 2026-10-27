import { expect, test } from "@playwright/test";
import {
  allocateBrowserRuntime,
  createBrowserTestIdentity,
  createBrowserRuntimeLifecycle,
} from "../support/browser-runtime-lifecycle.mjs";

test("AI-BROWSER-007 default allocations are unique and explicit overrides remain deterministic", async () => {
  const first = await allocateBrowserRuntime({
    environment: {},
    createRunId: () => "11111111111111111111111111111111",
    reservePort: async () => 18101,
  });
  const second = await allocateBrowserRuntime({
    environment: {},
    createRunId: () => "22222222222222222222222222222222",
    reservePort: async () => 18102,
  });

  expect(first).toEqual({
    runId: "11111111111111111111111111111111",
    composeProject: "hotjoes-browser-1111111111111111",
    postgresPassword: "browser-11111111111111111111111111111111",
    edgePort: "18101",
    baseUrl: "http://127.0.0.1:18101",
  });
  expect(second.composeProject).not.toBe(first.composeProject);
  expect(second.postgresPassword).not.toBe(first.postgresPassword);
  expect(second.edgePort).not.toBe(first.edgePort);
  expect(second.baseUrl).not.toBe(first.baseUrl);

  const explicit = await allocateBrowserRuntime({
    environment: {
      HOTJOES_BROWSER_RUN_ID: "controlled-run",
      HOTJOES_COMPOSE_PROJECT: "controlled-project",
      HOTJOES_POSTGRES_PASSWORD: "controlled-password",
      HOTJOES_EDGE_PORT: "19090",
      HOTJOES_BROWSER_BASE_URL: "http://127.0.0.1:19090",
    },
    createRunId: () => {
      throw new Error("A supplied run identity must not be replaced.");
    },
    reservePort: async () => {
      throw new Error("A supplied port must not be replaced.");
    },
  });

  expect(explicit).toEqual({
    runId: "controlled-run",
    composeProject: "controlled-project",
    postgresPassword: "controlled-password",
    edgePort: "19090",
    baseUrl: "http://127.0.0.1:19090",
  });
});

test("AI-BROWSER-004 test identities are unique, deterministic and independent of execution order", () => {
  const registration = {
    runId: "runtime-test-run",
    titlePath: ["registration", "records a vendor"],
    workerIndex: 0,
    repeatEachIndex: 0,
  };
  const community = {
    runId: "runtime-test-run",
    titlePath: ["community", "joins after registration"],
    workerIndex: 0,
    repeatEachIndex: 0,
  };

  const registrationFirst = createBrowserTestIdentity(registration);
  const communitySecond = createBrowserTestIdentity(community);
  const communityFirst = createBrowserTestIdentity(community);
  const registrationSecond = createBrowserTestIdentity(registration);

  expect(registrationFirst).toBe(registrationSecond);
  expect(communityFirst).toBe(communitySecond);
  expect(registrationFirst).not.toBe(communityFirst);
  expect(registrationFirst).toMatch(/^[a-z0-9-]+$/);

  expect(createBrowserTestIdentity({
    ...registration,
    repeatEachIndex: 1,
  })).not.toBe(registrationFirst);
});

test("AI-RUNTIME-006 lifecycle provisions in order and cleanup is idempotent", async () => {
  const calls: string[][] = [];
  const lifecycle = createBrowserRuntimeLifecycle({
    allocation: allocation(),
    runCompose: async (arguments_) => {
      calls.push([...arguments_]);
    },
    reportCleanupFailure: () => {
      throw new Error("Successful cleanup must not be reported as failed.");
    },
  });

  await lifecycle.provision();
  await lifecycle.cleanup();
  await lifecycle.cleanup();

  expect(calls).toEqual([
    ["down", "--volumes", "--remove-orphans"],
    ["up", "--build", "--wait", "--detach"],
    ["down", "--volumes", "--remove-orphans"],
  ]);
});

test("AI-RUNTIME-006 cleanup failure is reported without replacing the primary result", async () => {
  const calls: string[][] = [];
  const reports: unknown[] = [];
  let downCount = 0;
  const lifecycle = createBrowserRuntimeLifecycle({
    allocation: allocation(),
    runCompose: async (arguments_) => {
      calls.push([...arguments_]);
      if (arguments_[0] === "down" && ++downCount === 2) {
        throw new Error("synthetic unsafe cleanup detail");
      }
    },
    reportCleanupFailure: (report) => reports.push(report),
  });

  await lifecycle.provision();
  await lifecycle.cleanup();

  expect(reports).toEqual([{
    runId: "runtime-test-run",
    outcome: "cleanupFailed",
  }]);
  expect(JSON.stringify(reports)).not.toContain("synthetic unsafe cleanup detail");
  expect(calls.at(-1)).toEqual(["down", "--volumes", "--remove-orphans"]);
});

test("AI-RUNTIME-006 provisioning failure preserves the primary failure and attempts cleanup", async () => {
  const calls: string[][] = [];
  const primaryFailure = new Error("synthetic provisioning failure");
  const lifecycle = createBrowserRuntimeLifecycle({
    allocation: allocation(),
    runCompose: async (arguments_) => {
      calls.push([...arguments_]);
      if (arguments_[0] === "up") throw primaryFailure;
    },
    reportCleanupFailure: () => undefined,
  });

  await expect(lifecycle.provision()).rejects.toBe(primaryFailure);
  expect(calls).toEqual([
    ["down", "--volumes", "--remove-orphans"],
    ["up", "--build", "--wait", "--detach"],
    ["down", "--volumes", "--remove-orphans"],
  ]);
});

function allocation() {
  return {
    runId: "runtime-test-run",
    composeProject: "hotjoes-browser-runtime-test",
    postgresPassword: "synthetic-password",
    edgePort: "18103",
    baseUrl: "http://127.0.0.1:18103",
  };
}
