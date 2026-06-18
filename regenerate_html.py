#!/usr/bin/env python3
"""
Convert Markdown to HTML with PROPER styling for readable PDFs
"""

import os
from pathlib import Path

def markdown_to_html_fixed(md_file):
    """Convert markdown to HTML with readable styling"""
    try:
        import markdown2

        with open(md_file, 'r', encoding='utf-8') as f:
            md_content = f.read()

        html = markdown2.markdown(
            md_content,
            extras=['fenced-code-blocks', 'tables', 'toc']
        )

        # FIXED styling with PROPER contrast
        styled_html = f"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>{os.path.basename(md_file)}</title>
            <style>
                * {{
                    margin: 0;
                    padding: 0;
                }}

                html, body {{
                    background-color: #ffffff;
                    color: #000000;
                    font-family: 'Segoe UI', 'Arial', sans-serif;
                    line-height: 1.6;
                }}

                body {{
                    padding: 40px;
                    max-width: 1000px;
                    margin: 0 auto;
                    background: white;
                    color: black;
                }}

                h1 {{
                    color: #1a1a1a;
                    border-bottom: 3px solid #0066cc;
                    padding: 15px 0;
                    margin: 30px 0 20px 0;
                    font-size: 28px;
                    font-weight: bold;
                }}

                h2 {{
                    color: #1a1a1a;
                    border-left: 4px solid #0066cc;
                    padding-left: 15px;
                    margin: 25px 0 15px 0;
                    font-size: 22px;
                    font-weight: bold;
                    page-break-after: avoid;
                }}

                h3 {{
                    color: #2a2a2a;
                    margin: 15px 0 10px 0;
                    font-size: 18px;
                    font-weight: bold;
                    page-break-after: avoid;
                }}

                h4 {{
                    color: #3a3a3a;
                    margin: 12px 0 8px 0;
                    font-size: 16px;
                    font-weight: bold;
                }}

                p {{
                    margin: 12px 0;
                    color: #000000;
                    font-size: 14px;
                }}

                a {{
                    color: #0066cc;
                    text-decoration: underline;
                }}

                a:visited {{
                    color: #004499;
                }}

                code {{
                    background-color: #f5f5f5;
                    color: #000000;
                    padding: 2px 6px;
                    border-radius: 3px;
                    font-family: 'Courier New', monospace;
                    font-size: 13px;
                    border: 1px solid #ddd;
                }}

                pre {{
                    background-color: #f8f8f8;
                    color: #000000;
                    padding: 15px;
                    border-radius: 5px;
                    overflow-x: auto;
                    font-family: 'Courier New', monospace;
                    font-size: 12px;
                    border: 1px solid #ddd;
                    page-break-inside: avoid;
                    margin: 15px 0;
                    line-height: 1.4;
                }}

                pre code {{
                    background: none;
                    border: none;
                    padding: 0;
                    color: #000000;
                }}

                table {{
                    border-collapse: collapse;
                    width: 100%;
                    margin: 20px 0;
                    font-size: 13px;
                    page-break-inside: avoid;
                    border: 1px solid #ccc;
                }}

                th {{
                    background-color: #0066cc;
                    color: #ffffff;
                    font-weight: bold;
                    padding: 12px;
                    text-align: left;
                    border: 1px solid #0066cc;
                }}

                td {{
                    border: 1px solid #ccc;
                    padding: 10px;
                    color: #000000;
                }}

                tr:nth-child(even) {{
                    background-color: #f9f9f9;
                }}

                tr:nth-child(odd) {{
                    background-color: #ffffff;
                }}

                strong {{
                    color: #000000;
                    font-weight: bold;
                }}

                em {{
                    color: #000000;
                    font-style: italic;
                }}

                blockquote {{
                    border-left: 4px solid #0066cc;
                    padding-left: 15px;
                    margin: 15px 0;
                    color: #333333;
                    background-color: #f9f9f9;
                    padding: 10px 15px;
                    page-break-inside: avoid;
                    border-radius: 3px;
                }}

                ul {{
                    margin: 12px 0;
                    padding-left: 30px;
                    color: #000000;
                }}

                ol {{
                    margin: 12px 0;
                    padding-left: 30px;
                    color: #000000;
                }}

                li {{
                    margin: 6px 0;
                    color: #000000;
                }}

                .toc {{
                    background-color: #f0f0f0;
                    border: 1px solid #ccc;
                    padding: 15px;
                    border-radius: 5px;
                    page-break-inside: avoid;
                    margin: 20px 0;
                    color: #000000;
                }}

                .toc ul {{
                    list-style: none;
                    padding-left: 10px;
                }}

                .toc li {{
                    margin: 4px 0;
                    color: #000000;
                }}

                .toc a {{
                    color: #0066cc;
                }}

                hr {{
                    margin: 20px 0;
                    border: none;
                    border-top: 2px solid #0066cc;
                }}

                @page {{
                    size: A4;
                    margin: 1in;
                }}

                @media print {{
                    body {{
                        background: white;
                        color: black;
                    }}
                }}
            </style>
        </head>
        <body>
        {html}
        </body>
        </html>
        """
        return styled_html
    except Exception as e:
        print(f"Error converting markdown: {e}")
        return None

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
    print("📄 MARKDOWN → HTML (WITH PROPER READABLE STYLING)")
    print("=" * 70)
    print("\nRegenerating HTML files with improved CSS styling...\n")

    successful = 0
    for filename in files:
        md_file = os.path.join(docs_dir, filename)
        html_file = md_file.replace('.md', '.html')

        if os.path.exists(md_file):
            html = markdown_to_html_fixed(md_file)
            if html:
                with open(html_file, 'w', encoding='utf-8') as f:
                    f.write(html)
                print(f"  ✓ {filename} → {os.path.basename(html_file)}")
                successful += 1
            else:
                print(f"  ✗ {filename}")
        else:
            print(f"  ✗ Not found: {filename}")

    print("\n" + "=" * 70)
    print(f"✓ Generated: {successful}/{len(files)} HTML files")
    print("=" * 70)
    print("\n✅ HTML files regenerated with READABLE styling!")
    print("\nNext step: Convert HTML to PDF with proper colors\n")

if __name__ == "__main__":
    main()
