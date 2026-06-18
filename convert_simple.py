#!/usr/bin/env python3
"""
Simple Markdown to PDF Converter using Weasyprint
"""

import os
import sys
import subprocess
from pathlib import Path

def install_packages():
    """Install required packages"""
    packages = ['markdown2', 'weasyprint']

    for package in packages:
        try:
            __import__(package.replace('-', '_'))
        except ImportError:
            print(f"  📦 Installing {package}...")
            subprocess.check_call([sys.executable, "-m", "pip", "install", package, "-q"])

def markdown_to_html(md_file):
    """Convert markdown to HTML"""
    try:
        import markdown2

        with open(md_file, 'r', encoding='utf-8') as f:
            md_content = f.read()

        # Markdown options
        html = markdown2.markdown(
            md_content, 
            extras=['fenced-code-blocks', 'tables', 'toc']
        )

        # Styled HTML wrapper
        styled_html = f"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <style>
                @page {{
                    size: A4;
                    margin: 0.75in;
                    @bottom-center {{
                        content: "Page " counter(page);
                        font-size: 10pt;
                        color: #999;
                    }}
                }}

                body {{
                    font-family: 'Segoe UI', 'Arial', sans-serif;
                    line-height: 1.6;
                    color: #333;
                    background: white;
                }}

                h1 {{
                    color: #2c3e50;
                    border-bottom: 3px solid #3498db;
                    padding-bottom: 10px;
                    margin-top: 30px;
                    page-break-after: avoid;
                }}

                h2 {{
                    color: #34495e;
                    border-left: 4px solid #3498db;
                    padding-left: 10px;
                    margin-top: 25px;
                    page-break-after: avoid;
                }}

                h3 {{
                    color: #7f8c8d;
                    page-break-after: avoid;
                }}

                p {{
                    margin: 12px 0;
                }}

                code {{
                    background: #f4f4f4;
                    padding: 2px 6px;
                    border-radius: 3px;
                    font-family: 'Courier New', monospace;
                    font-size: 0.9em;
                }}

                pre {{
                    background: #2c3e50;
                    color: #ecf0f1;
                    padding: 15px;
                    border-radius: 5px;
                    overflow-x: auto;
                    font-family: 'Courier New', monospace;
                    font-size: 0.85em;
                    page-break-inside: avoid;
                    margin: 15px 0;
                }}

                table {{
                    border-collapse: collapse;
                    width: 100%;
                    margin: 20px 0;
                    font-size: 0.9em;
                    page-break-inside: avoid;
                }}

                th, td {{
                    border: 1px solid #ddd;
                    padding: 10px;
                    text-align: left;
                }}

                th {{
                    background: #3498db;
                    color: white;
                    font-weight: bold;
                }}

                tr:nth-child(even) {{
                    background: #f9f9f9;
                }}

                strong {{
                    color: #e74c3c;
                    font-weight: bold;
                }}

                em {{
                    color: #27ae60;
                    font-style: italic;
                }}

                blockquote {{
                    border-left: 4px solid #3498db;
                    padding-left: 15px;
                    margin-left: 0;
                    color: #7f8c8d;
                    background: #f9f9f9;
                    padding: 10px 15px;
                    page-break-inside: avoid;
                }}

                a {{
                    color: #3498db;
                    text-decoration: none;
                }}

                a:hover {{
                    text-decoration: underline;
                }}

                ul, ol {{
                    margin: 12px 0;
                    padding-left: 30px;
                }}

                li {{
                    margin: 6px 0;
                }}

                .toc {{
                    background: #f0f0f0;
                    padding: 15px;
                    border-radius: 5px;
                    page-break-inside: avoid;
                    margin: 20px 0;
                }}

                .toc ul {{
                    list-style: none;
                }}

                .toc li {{
                    margin: 4px 0;
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
        print(f"    ✗ Markdown conversion error: {e}")
        return None

def html_to_pdf(html_content, output_file):
    """Convert HTML to PDF using Weasyprint"""
    try:
        from weasyprint import HTML, CSS
        from io import BytesIO

        # Convert HTML string to PDF
        HTML(string=html_content).write_pdf(output_file)
        return True
    except Exception as e:
        print(f"    ✗ PDF conversion error: {e}")
        return False

def convert_file(md_file, output_pdf=None):
    """Convert single markdown file to PDF"""

    if not os.path.exists(md_file):
        print(f"    ✗ File not found: {md_file}")
        return False

    if output_pdf is None:
        output_pdf = os.path.splitext(md_file)[0] + '.pdf'

    filename = os.path.basename(md_file)
    print(f"  ✓ Converting: {filename}")

    # Convert MD → HTML
    html = markdown_to_html(md_file)
    if not html:
        return False

    # Convert HTML → PDF
    if html_to_pdf(html, output_pdf):
        size_mb = os.path.getsize(output_pdf) / (1024 * 1024)
        print(f"    ✓ {filename} → {os.path.basename(output_pdf)} ({size_mb:.2f} MB)")
        return True
    return False

def main():
    """Main function"""
    docs_dir = r"C:\Users\cfior\source\repos\HMISolution\docs"

    files = [
        "ARCHITECTURE_COMPLETE_CORRECTED.md",
        "CODE_DEEP_DIVE.md",
        "CORRECTIONS_SUMMARY.md",
        "EXECUTIVE_SUMMARY.md",
        "VISUAL_PRESENTATION.md",
    ]

    print("\n" + "=" * 70)
    print("📄 MARKDOWN → PDF CONVERSION")
    print("=" * 70)

    print("\n📦 Installing packages...")
    install_packages()

    print("\n🚀 Converting files...\n")

    successful = 0
    for filename in files:
        md_file = os.path.join(docs_dir, filename)
        pdf_file = os.path.join(docs_dir, filename.replace('.md', '.pdf'))

        if convert_file(md_file, pdf_file):
            successful += 1
        else:
            print(f"    ✗ Failed: {filename}")

    # Summary
    print("\n" + "=" * 70)
    print(f"✓ Converted: {successful}/{len(files)}")
    print(f"📍 Location: {docs_dir}")
    print("=" * 70 + "\n")

    return successful == len(files)

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)
