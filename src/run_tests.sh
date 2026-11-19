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
	report_files=$(find ${base_dir} -name "coverage.cobertura.xml")
	# TODO: Handle multiple report files
	report_file=${report_files[0]}
	reportgenerator -reports:"$report_file" -targetdir:"coveragereport" -reporttypes:Html
else
	echo "Running tests without coverage"
	dotnet test
fi
