# Claude Code Injector for GitHub

A browser-based tool that injects a Claude Code assistant panel into GitHub pages. Perfect for analyzing projects, extracting data, and interacting with GitHub repositories directly from your browser.

## Features

- 🔥 **Instant Injection**: Paste into browser console and start using immediately
- 📊 **Smart Page Detection**: Automatically detects GitHub page types (projects, issues, PRs)
- 📋 **Quick Actions**: Pre-built commands for common tasks
- 🎨 **GitHub-themed UI**: Seamlessly integrates with GitHub's design
- 🔌 **WebSocket Bridge**: Optional local bridge for enhanced Claude integration
- 🖱️ **Draggable Panel**: Position anywhere on the page
- 💾 **Data Extraction**: Export structured data from GitHub pages

## Quick Start

### Basic Usage (Standalone Mode)

1. Open any GitHub page in your browser
2. Open DevTools Console (F12 or Cmd+Option+J on Mac)
3. Copy the contents of `claude-code-injector.js`
4. Paste into the console and press Enter
5. The Claude Code panel will appear in the bottom-right corner

### Available Commands

Type these commands in the Claude Code input box:

- `/context` or `/ctx` - Show current page context and metadata
- `/items` or `/list` - List all project items on the page
- `/extract` - Extract structured data from the page (tables, cards, links)
- `/dom` - Show a snippet of the page's DOM structure
- `/help` - Display all available commands

### Quick Action Buttons

- **📄 Analyze Page** - Get context about the current GitHub page
- **📋 List Items** - Show all project items or issues
- **📊 Extract Data** - Export structured data as JSON
- **🔗 Find PRs** - Show help for finding pull requests

## Advanced Usage: WebSocket Bridge

For full Claude integration, run a local WebSocket bridge:

### Option 1: Using process_monitor.py (recommended)

```bash
python3 process_monitor.py
```

This starts a WebSocket server on `ws://localhost:9999` that the injector will automatically connect to.

### Option 2: Custom Bridge

Create your own WebSocket server that:
- Listens on port 9999
- Accepts messages with format: `{type: 'query', text: string, context: object}`
- Responds with: `{type: 'response', content: string}`

## Examples

### Analyzing a GitHub Project

1. Navigate to a GitHub project board
2. Inject the script
3. Type `/items` to see all tasks
4. Type `/extract` to get structured JSON data

### Extracting Issue Data

1. Navigate to repository issues page
2. Inject the script
3. Type `/extract` to get all issues with links
4. Copy the JSON output for further processing

### Custom Queries (with bridge)

When connected to a bridge with Claude integration:

```
"Summarize all open issues"
"List all PRs from this week"
"What's the status of task #42?"
```

## Page Type Detection

The injector automatically detects:

- **Project Boards**: Extracts project name, items, and statuses
- **Issues List**: Identifies issue context
- **Pull Requests**: Detects PR pages
- **Other Pages**: Generic GitHub page handling

## UI Features

### Draggable Panel

Click and drag the header to reposition the panel anywhere on the page.

### Minimize/Maximize

Click the `−` button to minimize the panel. Click `+` to expand it again.

### Connection Status

The status indicator shows:
- 🟢 Green: Connected to local bridge
- 🔴 Red: Standalone mode (no bridge)

## Customization

Edit the configuration at the top of the script:

```javascript
const CLAUDE_API_ENDPOINT = 'https://api.anthropic.com/v1/messages';
const WS_BRIDGE = 'ws://localhost:9999';  // Change port if needed
```

## Security Notes

- This script runs in your browser's context
- It can only access the current GitHub page you're viewing
- No data is sent externally unless you're connected to a local bridge
- The WebSocket bridge runs locally on your machine

## Browser Compatibility

- ✅ Chrome/Chromium 90+
- ✅ Firefox 88+
- ✅ Edge 90+
- ✅ Safari 14+

## Troubleshooting

### Panel doesn't appear
- Check browser console for errors
- Ensure you're on a GitHub page
- Try refreshing the page and re-injecting

### WebSocket connection fails
- Ensure `process_monitor.py` is running
- Check that port 9999 is not blocked by firewall
- The script works fine in standalone mode without the bridge

### Commands not working
- Make sure to include the `/` prefix for commands
- Check that you're on a supported GitHub page type
- Try `/help` to see available commands

## Integration with Pipe Trades CLI

This tool is part of the Strategickhaos Pipe Trades CLI ecosystem and can be used to:

- Monitor GitHub project boards for field task tracking
- Extract issue data for job coordination
- Analyze PR status for deployment planning
- Quick project overview during field operations

## License

Part of the Strategickhaos Pipe Trades CLI project.
