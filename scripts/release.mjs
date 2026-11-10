#!/usr/bin/env node
/**
 * Local release for the Imazeo .NET SDK.
 *
 *   $env:RELEASE_MODE="true"; npm run release
 *
 * Interactive bump + GitHub Release via release-it.
 * Syncs package.json → Imazeo.csproj + ImazeoVersion.cs, then commit/tag/push.
 * NuGet publish is handled by .github/workflows/publish.yml (OIDC).
 */

import { spawnSync } from 'node:child_process';
import { existsSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const releaseIt = join(root, 'node_modules', 'release-it', 'bin', 'release-it.js');

function fail(message) {
  console.error(`release: ${message}`);
  process.exit(1);
}

function run(command, args) {
  const result = spawnSync(command, args, {
    cwd: root,
    stdio: 'inherit',
    shell: false,
    env: process.env,
  });
  if (result.error) {
    fail(`failed to start ${command}: ${result.error.message}`);
  }
  if (result.status !== 0) process.exit(result.status ?? 1);
}

if (process.env.GITHUB_ACTIONS === 'true') {
  fail('CI publish is handled by .github/workflows/publish.yml');
}

if (process.env.RELEASE_MODE !== 'true') {
  fail('set RELEASE_MODE=true (PowerShell: $env:RELEASE_MODE="true")');
}

if (!existsSync(releaseIt)) {
  fail('release-it is not installed; run npm install');
}

run(process.execPath, [releaseIt]);
