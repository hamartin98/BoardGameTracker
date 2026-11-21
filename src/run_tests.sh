#!/bin/bash

while getopts c: opts; do
	case ${opts} in
	c) COVERAGE=${OPTARG} ;;
	esac
done

if [ "$COVERAGE" = true ]; then
	echo "Running tests with coverage"
	base_dir=./TestResults
	rm -rf $base_dir/*
	dotnet test --collect:"XPlat Code Coverage" --settings:"tests.runsettings"
	# One report per test project (unit and integration tests); reportgenerator merges them
	reportgenerator -reports:"${base_dir}/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
else
	echo "Running tests without coverage"
	dotnet test
fi
