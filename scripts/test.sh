#!/bin/bash
set -e

echo "Starting..."
mkdir myfolder   # If this fails, the script stops here
cd myfolder      # Won't execute if `mkdir` failed
echo "Inside folder"