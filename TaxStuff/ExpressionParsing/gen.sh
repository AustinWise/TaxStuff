#!/usr/bin/env bash

# To install dependencies for this script on Debian-like systems, run:
#   apt install antlr4

set -euo pipefail
set -x

cd -- "$( dirname -- "${BASH_SOURCE[0]}" )"

if [ ! -d .venv ]; then
    python -m venv .venv
fi
. .venv/bin/activate


pip install antlr4-tools

env ANTLR4_TOOLS_ANTLR_VERSION=4.13.2 antlr4 Expression.g4 -Dlanguage=CSharp -package TaxStuff.ExpressionParsing -visitor -no-listener
