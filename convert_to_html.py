#!/usr/bin/env python3
"""
Convert Markdown to PDF using an online service + local HTML conversion
"""

import os
import requests
import json
from pathlib import Path

def convert_with_online_service(md_file):
    """Use an online markdown to HTML service"""
    try:
        with open(md_file, 'r', encoding='utf-8') as f:
            md_content = f.read()

        # Try using markdown2 which we already have
        import markdown2

        html = markdown2.markdown(
            md_content,
            extras=['fenced-code-blocks', 'tables', 'toc']
        )

        styled_html = f"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>{os.path.basename(md_file)}</title>
            <style>
                body {{
                    font-family: 'Segoe UI', Arial, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    max-width: 900px;
                    margin: 0 auto;
                    padding: 20px;
                }}
                h1 {{ color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px; }}
                h2 {{ color: #34495e; border-left: 4px solid #3498db; padding-left: 10px; margin-top: 25px; }}
                h3 {{ color: #7f8c8d; }}
                code {{ background: #f4f4f4; padding: 2px 6px; border-radius: 3px; }}
                pre {{ background: #2c3e50; color: #ecf0f1; padding: 15px; border-radius: 5px; overflow-x: auto; }}
                table {{ border-collapse: collapse; width: 100%; margin: 20px 0; }}
                th, td {{ border: 1px solid #ddd; padding: 12px; text-align: left; }}
                th {{ background: #3498db; color: white; }}
                tr:nth-child(even) {{ background: #f9f9f9; }}
                a {{ color: #3498db; }}
            </style>
        </head>
        <body>
        {html}
        </body>
        </html>
        """

        html_file = md_file.replace('.md', '.html')
        with open(html_file, 'w', encoding='utf-8') as f:
            f.write(styled_html)

        return html_file, True
    except Exception as e:
        print(f"Error: {e}")
        return None, False

def main():
    docs_dir = r"C:\Users\cfior\source\repos\HMISolution\docs"

    files = [
        "ARCHITECTURE_COMPLETE_CORRECTED.md",
        "CODE_DEEP_DIVE.md",
        "CORRECTIONS_SUMMARY.md",
        "EXECUTIVE_SUMMARY.md",
        "VISUAL_PRESENTATION.md",
    ]

    print("\n" + "=" * 70)
    print("📄 MARKDOWN → HTML CONVERSION")
    print("=" * 70)
    print("\nNote: Generated HTML files can be opened in browser and printed as PDF\n")

    successful = 0
    for filename in files:
        md_file = os.path.join(docs_dir, filename)

        if os.path.exists(md_file):
            html_file, success = convert_with_online_service(md_file)
            if success:
                print(f"  ✓ {filename}")
                print(f"    → HTML: {os.path.basename(html_file)}")
                successful += 1
            else:
                print(f"  ✗ {filename}")
        else:
            print(f"  ✗ Not found: {filename}")

    print("\n" + "=" * 70)
    print(f"✓ Created: {successful}/{len(files)} HTML files")
    print("=" * 70)
    print("\n📝 To convert HTML to PDF:")
    print("  1. Open HTML file in browser (Edge, Chrome, Firefox)")
    print("  2. Press Ctrl+P (Print)")
    print("  3. Select 'Save as PDF'")
    print("  4. Done! ✓\n")

if __name__ == "__main__":
    main()
