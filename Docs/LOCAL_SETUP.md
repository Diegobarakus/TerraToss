# TerraToss — Local Setup

This file documents the current Windows development machine.

## Project path

```text
C:\Coding\Games\TerraToss\Dev\TerraToss
```

## Start UnityMCP manually

UnityMCP currently uses a dedicated Python environment because `uvx` encountered a Windows access-denied error while installing `pywin32`.

Open PowerShell and run:

```powershell
$McpEnv = "$env:LOCALAPPDATA\MCPForUnity\10.1.0"

& "$McpEnv\Scripts\mcp-for-unity.exe" `
  --transport http `
  --http-url http://127.0.0.1:8080 `
  --project-scoped-tools
```

Keep that PowerShell window open.

Then:
1. Open Unity and TerraToss.
2. Open another PowerShell window.
3. Run:

```powershell
cd "C:\Coding\Games\TerraToss\Dev\TerraToss"
claude
```

4. Use `/mcp` and confirm:

```text
UnityMCP · connected
```

Do not start a second MCP server on port 8080.