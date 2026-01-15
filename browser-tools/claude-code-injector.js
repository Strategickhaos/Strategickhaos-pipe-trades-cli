// ============================================================================
// STRATEGICKHAOS - Claude Code Injector for GitHub Projects
// Paste this into DevTools Console on any GitHub page
// ============================================================================

(function() {
    'use strict';
    
    // Configuration
    const WS_BRIDGE = 'ws://localhost:9999';  // Local bridge for Claude Code
    
    // Create the Claude Code panel
    const panel = document.createElement('div');
    panel.id = 'claude-code-panel';
    panel.innerHTML = `
        <style>
            #claude-code-panel {
                position: fixed;
                bottom: 20px;
                right: 20px;
                width: 400px;
                max-height: 500px;
                background: #0d1117;
                border: 1px solid #30363d;
                border-radius: 12px;
                box-shadow: 0 8px 32px rgba(0,0,0,0.4);
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
                z-index: 999999;
                overflow: hidden;
            }
            #claude-header {
                background: linear-gradient(135deg, #238636, #1f6feb);
                padding: 12px 16px;
                display: flex;
                justify-content: space-between;
                align-items: center;
                cursor: move;
            }
            #claude-header h3 {
                margin: 0;
                color: white;
                font-size: 14px;
                display: flex;
                align-items: center;
                gap: 8px;
            }
            #claude-minimize {
                background: none;
                border: none;
                color: white;
                cursor: pointer;
                font-size: 18px;
                padding: 0 4px;
            }
            #claude-body {
                padding: 12px;
                max-height: 350px;
                overflow-y: auto;
            }
            #claude-output {
                background: #161b22;
                border: 1px solid #30363d;
                border-radius: 8px;
                padding: 12px;
                margin-bottom: 12px;
                min-height: 100px;
                max-height: 200px;
                overflow-y: auto;
                font-size: 13px;
                color: #c9d1d9;
                white-space: pre-wrap;
                font-family: 'JetBrains Mono', monospace;
            }
            #claude-input-container {
                display: flex;
                gap: 8px;
            }
            #claude-input {
                flex: 1;
                background: #161b22;
                border: 1px solid #30363d;
                border-radius: 6px;
                padding: 10px 12px;
                color: #c9d1d9;
                font-size: 13px;
                resize: none;
            }
            #claude-input:focus {
                outline: none;
                border-color: #58a6ff;
            }
            #claude-send {
                background: #238636;
                border: none;
                border-radius: 6px;
                padding: 10px 16px;
                color: white;
                cursor: pointer;
                font-weight: 600;
                transition: background 0.2s;
            }
            #claude-send:hover {
                background: #2ea043;
            }
            #claude-send:disabled {
                background: #484f58;
                cursor: not-allowed;
            }
            .claude-status {
                font-size: 11px;
                color: #8b949e;
                margin-top: 8px;
                display: flex;
                align-items: center;
                gap: 6px;
            }
            .claude-status .dot {
                width: 6px;
                height: 6px;
                border-radius: 50%;
                background: #da3633;
            }
            .claude-status .dot.connected {
                background: #3fb950;
            }
            .claude-actions {
                display: flex;
                gap: 6px;
                margin-top: 8px;
                flex-wrap: wrap;
            }
            .claude-action-btn {
                background: #21262d;
                border: 1px solid #30363d;
                border-radius: 4px;
                padding: 4px 8px;
                color: #8b949e;
                font-size: 11px;
                cursor: pointer;
            }
            .claude-action-btn:hover {
                background: #30363d;
                color: #c9d1d9;
            }
            .minimized #claude-body {
                display: none;
            }
        </style>
        
        <div id="claude-header">
            <h3>🔥 Claude Code</h3>
            <button id="claude-minimize">−</button>
        </div>
        
        <div id="claude-body">
            <div id="claude-output">Ready. Ask me about this GitHub project or paste code for analysis.</div>
            
            <div id="claude-input-container">
                <textarea id="claude-input" rows="2" placeholder="Ask Claude..."></textarea>
                <button id="claude-send">Send</button>
            </div>
            
            <div class="claude-actions">
                <button class="claude-action-btn" data-action="analyze-page">📄 Analyze Page</button>
                <button class="claude-action-btn" data-action="list-items">📋 List Items</button>
                <button class="claude-action-btn" data-action="extract-data">📊 Extract Data</button>
                <button class="claude-action-btn" data-action="find-prs">🔗 Find PRs</button>
            </div>
            
            <div class="claude-status">
                <span class="dot" id="claude-ws-status"></span>
                <span id="claude-status-text">Connecting to bridge...</span>
            </div>
        </div>
    `;
    
    document.body.appendChild(panel);
    
    // Elements
    const output = document.getElementById('claude-output');
    const input = document.getElementById('claude-input');
    const sendBtn = document.getElementById('claude-send');
    const minimizeBtn = document.getElementById('claude-minimize');
    const statusDot = document.getElementById('claude-ws-status');
    const statusText = document.getElementById('claude-status-text');
    const header = document.getElementById('claude-header');
    
    // WebSocket connection to local bridge
    let ws = null;
    let isConnected = false;
    
    function connectWebSocket() {
        try {
            ws = new WebSocket(WS_BRIDGE);
            
            ws.onopen = () => {
                isConnected = true;
                statusDot.classList.add('connected');
                statusText.textContent = 'Connected to local bridge';
                log('✅ Connected to StrategicKhaos bridge');
            };
            
            ws.onclose = () => {
                isConnected = false;
                statusDot.classList.remove('connected');
                statusText.textContent = 'Bridge disconnected. Using fallback mode.';
                setTimeout(connectWebSocket, 5000);
            };
            
            ws.onmessage = (e) => {
                const data = JSON.parse(e.data);
                if (data.type === 'response') {
                    log(data.content);
                }
            };
            
            ws.onerror = () => {
                statusText.textContent = 'Bridge unavailable. Running in standalone mode.';
            };
        } catch (err) {
            statusText.textContent = 'Running in standalone mode (no bridge)';
        }
    }
    
    // Logging
    function log(message) {
        output.textContent = message;
        output.scrollTop = output.scrollHeight;
    }
    
    // Get page context
    function getPageContext() {
        const context = {
            url: window.location.href,
            title: document.title,
            type: 'unknown'
        };
        
        // Detect GitHub page type
        if (location.pathname.includes('/projects/')) {
            context.type = 'project';
            context.projectName = (document.querySelector('[data-testid="project-name"]')?.textContent?.trim() 
                               || document.querySelector('.js-project-name')?.textContent?.trim()
                               || 'Unknown Project');
            
            // Get project items
            const items = [];
            document.querySelectorAll('[data-testid="table-row"], .js-project-column-card').forEach(el => {
                items.push({
                    title: el.querySelector('[data-testid="table-cell-title"], .js-project-card-title')?.textContent?.trim(),
                    status: el.querySelector('[data-testid="table-cell-status"]')?.textContent?.trim()
                });
            });
            context.items = items.filter(i => i.title);
        }
        else if (location.pathname.includes('/issues')) {
            context.type = 'issues';
        }
        else if (location.pathname.includes('/pull')) {
            context.type = 'pull_request';
        }
        
        return context;
    }
    
    // Process commands
    async function processCommand(text) {
        const ctx = getPageContext();
        
        // Quick actions
        if (text.startsWith('/')) {
            const cmd = text.slice(1).toLowerCase();
            
            if (cmd === 'context' || cmd === 'ctx') {
                return JSON.stringify(ctx, null, 2);
            }
            
            if (cmd === 'items' || cmd === 'list') {
                if (ctx.items && ctx.items.length) {
                    return ctx.items.map((i, n) => `${n+1}. ${i.title} [${i.status || 'No status'}]`).join('\n');
                }
                return 'No items found on this page.';
            }
            
            if (cmd === 'extract') {
                return extractPageData();
            }
            
            if (cmd === 'dom') {
                return document.body.innerHTML.slice(0, 5000) + '...';
            }
            
            if (cmd === 'help') {
                return `Available commands:
/context - Show page context
/items - List project items
/extract - Extract structured data
/dom - Show DOM snippet
/help - Show this help`;
            }
        }
        
        // If connected to bridge, send there
        if (isConnected && ws) {
            ws.send(JSON.stringify({
                type: 'query',
                text: text,
                context: ctx
            }));
            return '⏳ Processing via local bridge...';
        }
        
        // Fallback: local processing
        return `📍 Page: ${ctx.type}
📋 Project: ${ctx.projectName || 'N/A'}
📊 Items: ${ctx.items?.length || 0}

Your query: "${text}"

[Bridge not connected - run process_monitor.py locally for full Claude integration]

Type /help for available offline commands.`;
    }
    
    // Extract data from page
    function extractPageData() {
        const data = {
            timestamp: new Date().toISOString(),
            url: location.href,
            title: document.title,
            tables: [],
            cards: [],
            links: []
        };
        
        // Extract tables
        document.querySelectorAll('table').forEach(table => {
            const rows = [];
            table.querySelectorAll('tr').forEach(tr => {
                const cells = Array.from(tr.querySelectorAll('td, th')).map(c => c.textContent.trim());
                if (cells.length) rows.push(cells);
            });
            if (rows.length) data.tables.push(rows);
        });
        
        // Extract cards/items
        document.querySelectorAll('[data-testid="table-row"], .js-project-card').forEach(el => {
            data.cards.push({
                title: el.querySelector('a, [data-testid*="title"]')?.textContent?.trim(),
                link: el.querySelector('a')?.href
            });
        });
        
        // Extract links
        document.querySelectorAll('a[href*="/issues/"], a[href*="/pull/"]').forEach(a => {
            data.links.push({
                text: a.textContent.trim().slice(0, 50),
                href: a.href
            });
        });
        
        return JSON.stringify(data, null, 2);
    }
    
    // Event handlers
    sendBtn.addEventListener('click', async () => {
        const text = input.value.trim();
        if (!text) return;
        
        sendBtn.disabled = true;
        log('⏳ Processing...');
        
        try {
            const response = await processCommand(text);
            log(response);
        } catch (err) {
            log('❌ Error: ' + err.message);
        }
        
        sendBtn.disabled = false;
        input.value = '';
    });
    
    input.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendBtn.click();
        }
    });
    
    minimizeBtn.addEventListener('click', () => {
        panel.classList.toggle('minimized');
        minimizeBtn.textContent = panel.classList.contains('minimized') ? '+' : '−';
    });
    
    // Quick action buttons
    document.querySelectorAll('.claude-action-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const action = btn.dataset.action;
            const commands = {
                'analyze-page': '/context',
                'list-items': '/items',
                'extract-data': '/extract',
                'find-prs': '/help'
            };
            input.value = commands[action] || '/help';
            sendBtn.click();
        });
    });
    
    // Draggable header
    let isDragging = false;
    let dragOffset = { x: 0, y: 0 };
    
    header.addEventListener('mousedown', (e) => {
        isDragging = true;
        dragOffset.x = e.clientX - panel.offsetLeft;
        dragOffset.y = e.clientY - panel.offsetTop;
    });
    
    document.addEventListener('mousemove', (e) => {
        if (!isDragging) return;
        panel.style.left = (e.clientX - dragOffset.x) + 'px';
        panel.style.top = (e.clientY - dragOffset.y) + 'px';
        panel.style.right = 'auto';
        panel.style.bottom = 'auto';
    });
    
    document.addEventListener('mouseup', () => {
        isDragging = false;
    });
    
    // Initialize
    connectWebSocket();
    log('🔥 Claude Code injected!\n\nCommands:\n/context - Page info\n/items - List items\n/extract - Get data\n/help - All commands');
    
})();
