gulp = require 'gulp'
args = require('yargs').argv
dotnet = require 'gulp-dotnet-utils'
sequence = require 'run-sequence'

pkg = require './package.json'

configuration = if args.debug then 'Debug' else 'Release'

gulp.task 'default', -> sequence 'build', 'test'

gulp.task 'build', ['restore'], ->
  dotnet.build configuration, ['Clean', 'Build'], toolsVersion: 14.0

gulp.task 'clean', -> dotnet.build configuration, ['Clean']

gulp.task 'test', -> dotnet.test [
  "tests/bin/#{configuration}/RDumont.NugetContentGenerator.Runtime.Tests.dll"
  ]

gulp.task 'restore', -> dotnet.exec 'nuget restore'

gulp.task 'pack', ->
  dotnet.nuget.pack 'src/package.nuspec', pkg.version,
    symbols: false
    configuration: configuration

gulp.task 'bump', -> dotnet.bump pkg.version
