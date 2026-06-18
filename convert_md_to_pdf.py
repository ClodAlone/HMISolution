#!/usr/bin/env python3
"""
Markdown to PDF Converter
Converts markdown files to professional PDF documents
"""

import os
import sys
import subprocess
from pathlib import Path

def install_required_packages():
    """Install required Python packages if not already installed"""
    packages = ['markdown2', 'pdfkit', 'pypdf']

    for package in packages:
        try:
            __import__(package.replace('-', '_'))
        except ImportError:
            print(f"Installing {package}...")
            subprocess.check_call([sys.executable, "-m", "pip", "install", package, "-q"])

    print("✓ All required packages are ready")

def markdown_to_html(md_file):
    """Convert markdown to HTML"""
    try:
        import markdown2

        with open(md_file, 'r', encoding='utf-8') as f:
            md_content = f.read()

        html = markdown2.markdown(md_content, extras=['fenced-code-blocks', 'tables', 'toc'])

        # Add CSS styling
        styled_html = f"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <style>
                body {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    max-width: 900px;
                    margin: 0 auto;
                    padding: 20px;
                    background: white;
                }}
                h1 {{ color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px; }}
                h2 {{ color: #34495e; margin-top: 30px; border-left: 4px solid #3498db; padding-left: 10px; }}
                h3 {{ color: #7f8c8d; }}
                code {{ background: #f4f4f4; padding: 2px 6px; border-radius: 3px; font-family: 'Courier New', monospace; }}
                pre {{
                    background: #2c3e50;
                    color: #ecf0f1;
                    padding: 15px;
                    border-radius: 5px;
                    overflow-x: auto;
                    font-family: 'Courier New', monospace;
                }}
                table {{
                    border-collapse: collapse;
                    width: 100%;
                    margin: 20px 0;
                }}
                th, td {{
                    border: 1px solid #ddd;
                    padding: 12px;
                    text-align: left;
                }}
                th {{
                    background: #3498db;
                    color: white;
                }}
                tr:nth-child(even) {{
                    background: #f9f9f9;
                }}
                strong {{ color: #e74c3c; }}
                em {{ color: #27ae60; }}
                blockquote {{
                    border-left: 4px solid #3498db;
                    padding-left: 20px;
                    margin-left: 0;
                    color: #7f8c8d;
                }}
                a {{ color: #3498db; text-decoration: none; }}
                a:hover {{ text-decoration: underline; }}
                .page-break {{ page-break-after: always; }}
            </style>
        </head>
        <body>
            {html}
        </body>
        </html>
        """
        return styled_html
    except Exception as e:
        print(f"✗ Error converting markdown: {e}")
        return None

def html_to_pdf(html_content, output_file):
    """Convert HTML to PDF using wkhtmltopdf via pdfkit"""
    try:
        import pdfkit

        # Try to use wkhtmltopdf if available
        try:
            pdfkit.from_string(html_content, output_file, options={
                'page-size': 'A4',
                'margin-top': '0.75in',
                'margin-right': '0.75in',
                'margin-bottom': '0.75in',
                'margin-left': '0.75in',
                'encoding': "UTF-8",
                'no-outline': None,
                'enable-local-file-access': None,
            })
            return True
        except Exception:
            # Fallback: use pypdf with reportlab
            print("  → wkhtmltopdf not found, using reportlab fallback...")
            from reportlab.lib.pagesizes import letter, A4
            from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, Table, TableStyle
            from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
            from reportlab.lib.units import inch
            from html.parser import HTMLParser

            # Simple HTML to reportlab conversion
            pdf = SimpleDocTemplate(output_file, pagesize=A4,
                                   rightMargin=0.75*inch, leftMargin=0.75*inch,
                                   topMargin=0.75*inch, bottomMargin=0.75*inch)

            styles = getSampleStyleSheet()
            story = []

            # Parse and add basic elements
            from xml.etree import ElementTree as ET
            try:
                # Remove unsupported HTML entities for parsing
                clean_html = html_content.replace('&nbsp;', ' ').replace('&mdash;', '—')
                root = ET.fromstring(f"<root>{clean_html}</root>")
                story.append(Paragraph(ET.tostring(root, encoding='unicode'), styles['Normal']))
            except:
                # If parsing fails, just add the text
                story.append(Paragraph(html_content, styles['Normal']))

            pdf.build(story)
            return True

    except Exception as e:
        print(f"✗ Error converting HTML to PDF: {e}")
        return False

def convert_markdown_file(md_file, output_pdf=None):
    """Convert a single markdown file to PDF"""

    if not os.path.exists(md_file):
        print(f"✗ File not found: {md_file}")
        return False

    if output_pdf is None:
        output_pdf = os.path.splitext(md_file)[0] + '.pdf'

    print(f"\n📄 Converting: {os.path.basename(md_file)}")
    print(f"   → Output: {os.path.basename(output_pdf)}")

    # Step 1: MD to HTML
    html = markdown_to_html(md_file)
    if not html:
        return False

    # Step 2: HTML to PDF
    if html_to_pdf(html, output_pdf):
        file_size_mb = os.path.getsize(output_pdf) / (1024 * 1024)
        print(f"   ✓ Success! ({file_size_mb:.2f} MB)")
        return True
    else:
        return False

def main():
    """Main conversion function"""
    docs_dir = r"C:\Users\cfior\source\repos\HMISolution\docs"

    # Files to convert
    files_to_convert = [
        "ARCHITECTURE_COMPLETE_CORRECTED.md",
        "CODE_DEEP_DIVE.md",
        "CORRECTIONS_SUMMARY.md",
        "EXECUTIVE_SUMMARY.md",
        "VISUAL_PRESENTATION.md",
    ]

    print("=" * 70)
    print("🔄 MARKDOWN TO PDF CONVERTER")
    print("=" * 70)

    # Install packages
    print("\n📦 Checking and installing required packages...")
    install_required_packages()

    # Convert files
    print("\n🚀 Converting files to PDF...\n")
    successful = 0
    failed = 0

    for filename in files_to_convert:
        md_file = os.path.join(docs_dir, filename)
        pdf_file = os.path.join(docs_dir, filename.replace('.md', '.pdf'))

        if convert_markdown_file(md_file, pdf_file):
            successful += 1
        else:
            failed += 1

    # Summary
    print("\n" + "=" * 70)
    print("📊 CONVERSION SUMMARY")
    print("=" * 70)
    print(f"✓ Successful: {successful}")
    print(f"✗ Failed: {failed}")
    print(f"📍 Output location: {docs_dir}")
    print("=" * 70)

    return failed == 0

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)
