#!/usr/bin/env node
/** Local pre-release checks. Dotnet test runs only when an SDK is available. */
import { spawnSync } from 'node:child_process';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');

function fail(message) {
  console.error(`pre-release-check: ${message}`);
  process.exit(1);
}

function run(command, args, { inherit = true } = {}) {
  const result = spawnSync(command, args, {
    cwd: root,
    stdio: inherit ? 'inherit' : 'pipe',
    encoding: 'utf8',
    shell: false,
    env: process.env,
  });
  if (result.error) fail(`failed to start ${command}: ${result.error.message}`);
  if (result.status !== 0) process.exit(result.status ?? 1);
  return result;
}

run('python', ['scripts/check-release.py']);

const sdks = spawnSync('dotnet', ['--list-sdks'], {
  cwd: root,
  encoding: 'utf8',
  shell: false,
  env: process.env,
});

if (sdks.error || sdks.status !== 0 || !(sdks.stdout || '').trim()) {
  console.log('pre-release-check: skipping dotnet test');
  process.exit(0);
}

run('dotnet', ['test', '-c', 'Release']);
