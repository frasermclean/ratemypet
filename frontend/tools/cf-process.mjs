/**
 * Cloudflare worker build conversion script.
 */

import fs from "node:fs";
import { join, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const root = resolve(fileURLToPath(import.meta.url), "../../");
const browser = resolve(root, "dist/browser");
const server = resolve(root, "dist/server");
const cloudflare = resolve(root, "dist/cloudflare");
const worker = resolve(cloudflare, "_worker.js");

console.log("Processing Cloudflare worker build...");

// copy browser and server files to cloudflare worker directories
fs.cpSync(browser, cloudflare, { recursive: true });
fs.cpSync(server, worker, { recursive: true });

// rename server.mjs to index.js
fs.renameSync(join(worker, "server.mjs"), join(worker, "index.js"));
