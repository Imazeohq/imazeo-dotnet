#!/usr/bin/env node
/** Keep Imazeo.csproj Version and ImazeoVersion.SdkVersion in sync after release-it bumps package.json. */
import { readFileSync, writeFileSync } from 'node:fs';
import { spawnSync } from 'node:child_process';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const version = JSON.parse(readFileSync(join(root, 'package.json'), 'utf8')).version;

const csprojPath = join(root, 'src', 'Imazeo', 'Imazeo.csproj');
const csproj = readFileSync(csprojPath, 'utf8');
const nextCsproj = csproj.replace(/<Version>[^<]+<\/Version>/, `<Version>${version}</Version>`);
if (!nextCsproj.includes(`<Version>${version}</Version>`)) {
  console.error('sync-sdk-version: failed to update Imazeo.csproj');
  process.exit(1);
}
writeFileSync(csprojPath, nextCsproj, 'utf8');

const versionCsPath = join(root, 'src', 'Imazeo', 'ImazeoVersion.cs');
const versionCs = readFileSync(versionCsPath, 'utf8');
const nextVersionCs = versionCs.replace(
  /SdkVersion\s*=\s*"[^"]+"/,
  `SdkVersion = "${version}"`,
);
if (!nextVersionCs.includes(`SdkVersion = "${version}"`)) {
  console.error('sync-sdk-version: failed to update ImazeoVersion.cs');
  process.exit(1);
}
writeFileSync(versionCsPath, nextVersionCs, 'utf8');

const add = spawnSync(
  'git',
  ['add', 'src/Imazeo/Imazeo.csproj', 'src/Imazeo/ImazeoVersion.cs', 'package.json'],
  {
    cwd: root,
    stdio: 'inherit',
    shell: false,
  },
);
if (add.status !== 0) process.exit(add.status ?? 1);
console.log(`sync-sdk-version: ${version}`);
