#!/usr/bin/env python3
"""
STRATEGICKHAOS - WebSocket Bridge for Claude Code Injector
==========================================================
Local bridge server that connects the browser-based Claude Code Injector
with backend processing and optional Claude API integration.

Usage:
    python3 process_monitor.py
    
Then inject claude-code-injector.js in your browser's DevTools console.
"""

import asyncio
import json
import sys
from datetime import datetime
from typing import Dict, Optional

try:
    import websockets
except ImportError:
    print("ERROR: websockets library not installed")
    print("Install with: pip3 install websockets")
    sys.exit(1)


# Configuration
WS_HOST = "localhost"
WS_PORT = 9999
MAX_RESPONSE_LENGTH = 5000


class ClaudeBridge:
    """WebSocket bridge for handling Claude Code Injector connections."""
    
    def __init__(self):
        self.active_connections = set()
        self.message_count = 0
        
    async def handle_client(self, websocket, path):
        """Handle incoming WebSocket connections from browser."""
        self.active_connections.add(websocket)
        client_addr = websocket.remote_address
        
        print(f"✅ Client connected: {client_addr}")
        print(f"📊 Active connections: {len(self.active_connections)}")
        
        try:
            # Send welcome message
            await websocket.send(json.dumps({
                "type": "response",
                "content": "✅ Connected to StrategicKhaos Bridge\n\nReady to process queries."
            }))
            
            async for message in websocket:
                await self.process_message(websocket, message)
                
        except websockets.exceptions.ConnectionClosed:
            print(f"❌ Client disconnected: {client_addr}")
        except Exception as e:
            print(f"⚠️  Error handling client {client_addr}: {e}")
        finally:
            self.active_connections.remove(websocket)
            print(f"📊 Active connections: {len(self.active_connections)}")
    
    async def process_message(self, websocket, message: str):
        """Process incoming messages from browser."""
        try:
            data = json.loads(message)
            self.message_count += 1
            
            if data.get("type") == "query":
                text = data.get("text", "")
                context = data.get("context", {})
                
                print(f"\n{'='*60}")
                print(f"📨 Query #{self.message_count}")
                print(f"{'='*60}")
                print(f"Text: {text}")
                print(f"Context: {context.get('type', 'unknown')} page")
                print(f"URL: {context.get('url', 'N/A')}")
                
                # Process the query
                response_text = await self.process_query(text, context)
                
                # Send response back to browser
                await websocket.send(json.dumps({
                    "type": "response",
                    "content": response_text
                }))
                
                print(f"✅ Response sent ({len(response_text)} chars)")
                
        except json.JSONDecodeError:
            error_msg = "❌ Invalid JSON message received"
            print(error_msg)
            await websocket.send(json.dumps({
                "type": "response",
                "content": error_msg
            }))
        except Exception as e:
            error_msg = f"❌ Error processing message: {str(e)}"
            print(error_msg)
            await websocket.send(json.dumps({
                "type": "response",
                "content": error_msg
            }))
    
    async def process_query(self, text: str, context: Dict) -> str:
        """
        Process user query with context.
        
        This is where you would integrate with Claude API or other services.
        For now, provides intelligent local responses.
        """
        
        # Simulate processing delay
        await asyncio.sleep(0.1)
        
        page_type = context.get("type", "unknown")
        url = context.get("url", "")
        
        # Smart responses based on query content
        query_lower = text.lower()
        
        if "summarize" in query_lower or "summary" in query_lower:
            if page_type == "project":
                items = context.get("items", [])
                item_count = len(items)
                return f"""📊 Project Summary
                
Total Items: {item_count}
Page Type: {page_type}
URL: {url}

This appears to be a GitHub project board. Use /items to see the full list.

[For AI-powered summaries, integrate Claude API key in process_monitor.py]
"""
            else:
                return f"""📄 Page Analysis

Type: {page_type}
URL: {url}

This is a {page_type} page on GitHub.

[For AI-powered analysis, integrate Claude API key in process_monitor.py]
"""
        
        if "status" in query_lower:
            items = context.get("items", [])
            if items:
                status_summary = {}
                for item in items:
                    status = item.get("status", "No status")
                    status_summary[status] = status_summary.get(status, 0) + 1
                
                summary_lines = [f"📊 Status Summary\n"]
                for status, count in status_summary.items():
                    summary_lines.append(f"{status}: {count} items")
                
                return "\n".join(summary_lines)
            else:
                return "No items found to analyze status."
        
        if "count" in query_lower or "how many" in query_lower:
            items = context.get("items", [])
            return f"📊 Found {len(items)} items on this page."
        
        # Default response
        return f"""🤖 Bridge Processing

Query: "{text}"
Page: {page_type}
Context Items: {len(context.get('items', []))}

This bridge is running in demo mode.

To enable full Claude AI integration:
1. Add your Anthropic API key to process_monitor.py
2. Uncomment the Claude API integration code
3. Restart the bridge

Current capabilities (without API key):
- Context extraction ✅
- Page type detection ✅
- Item counting ✅
- Basic query processing ✅

For advanced AI features, add Claude API integration.
"""
    
    async def start_server(self):
        """Start the WebSocket server."""
        print(f"""
{'='*60}
🔥 STRATEGICKHAOS - Claude Code Bridge
{'='*60}
WebSocket Server: ws://{WS_HOST}:{WS_PORT}
Status: Starting...
{'='*60}

Instructions:
1. Keep this terminal open
2. Open GitHub in your browser
3. Open DevTools Console (F12)
4. Paste claude-code-injector.js
5. The panel will auto-connect to this bridge

Press Ctrl+C to stop the server.
{'='*60}
""")
        
        async with websockets.serve(self.handle_client, WS_HOST, WS_PORT):
            print(f"✅ Server listening on ws://{WS_HOST}:{WS_PORT}")
            print(f"⏳ Waiting for browser connections...\n")
            await asyncio.Future()  # Run forever


# ============================================================================
# OPTIONAL: Claude API Integration
# ============================================================================
# 
# To enable full Claude AI capabilities, uncomment and configure below:
#
# import anthropic
# 
# ANTHROPIC_API_KEY = "your-api-key-here"
# 
# async def query_claude(text: str, context: Dict) -> str:
#     """Query Claude API with user text and page context."""
#     client = anthropic.Anthropic(api_key=ANTHROPIC_API_KEY)
#     
#     system_prompt = f"""You are an AI assistant helping analyze GitHub pages.
# 
# Current page context:
# - Type: {context.get('type')}
# - URL: {context.get('url')}
# - Items: {len(context.get('items', []))}
# 
# Provide concise, helpful responses about the GitHub page content."""
#     
#     message = client.messages.create(
#         model="claude-3-5-sonnet-20241022",
#         max_tokens=1024,
#         system=system_prompt,
#         messages=[{"role": "user", "content": text}]
#     )
#     
#     return message.content[0].text
#
# Then in process_query(), replace the smart responses with:
#     return await query_claude(text, context)
#
# ============================================================================


def main():
    """Main entry point."""
    print("Starting Claude Code Bridge...")
    
    bridge = ClaudeBridge()
    
    try:
        asyncio.run(bridge.start_server())
    except KeyboardInterrupt:
        print("\n\n⚠️  Server shutting down...")
        print(f"📊 Total messages processed: {bridge.message_count}")
        print("👋 Goodbye!")
    except Exception as e:
        print(f"\n❌ Server error: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
