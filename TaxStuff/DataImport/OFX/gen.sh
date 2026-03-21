#!/usr/bin/env bash

# To install dependencies for this script on Debian-like systems, run:
#   apt install mono-devel

set -euo pipefail

cd -- "$( dirname -- "${BASH_SOURCE[0]}" )"

xsd MyOfxWrapper.xsd /nologo /classes /namespace:TaxStuff.DataImport.OFX
