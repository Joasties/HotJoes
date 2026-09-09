# HotJoes Repository Instructions

## Repository Authority

The HotJoes repository is human-controlled.

AI agents may:

- read and inspect repository documentation and source code;
- inspect version-control state and history;
- analyse proposed changes;
- generate Change Requests, candidate documents, code, tests and patches outside this repository;
- create a disposable copy or isolated sandbox outside this repository;
- build and test proposed changes within that disposable environment; and
- report verification evidence to the human.

AI agents must not:

- create, modify, rename, move or delete files in this repository;
- apply approved Change Requests to controlled architecture documentation;
- apply generated or approved code or tests to the source tree;
- stage, commit, merge, rebase, push or otherwise alter repository Git state;
- treat human approval of a proposal as authority to apply it; or
- interpret requests such as “update,” “implement,” “apply,” or “continue” as repository-write authority.

All generated artefacts must be presented to the human as proposed candidates outside the HotJoes repository.

Only the human applies approved architecture documents, source code and test code to this repository.

Repository modification is permitted only when the human gives an explicit instruction that identifies:

1. the HotJoes repository as the write target;
2. the exact files or bounded change to apply; and
3. that direct repository modification is authorised for that specific task.

Approval of a CR, ADR, candidate document, code change or test change is not direct repository-write authority.

## Testing and Verification

The repository may be read to prepare builds and tests.

Where build, test, formatting or generation commands would write files, caches or build outputs inside the repository, the AI must first create a disposable copy or isolated sandbox outside the repository and run those commands there.

Test execution must not modify the controlled repository.

## Default Operating Mode

Repository modification authority: Read Only.

Candidate output authority: Proposal Only.

Human application gate: Required.
