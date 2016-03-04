'use strict';

var gulp = require('gulp');
var jsonutil = require('jsonutil');
var runSequence = require('run-sequence');
var del = require('del');
var wiredep = require('wiredep');
var Server = require('karma').Server;

var $ = require('gulp-load-plugins')({
  pattern: ['gulp-*']
});

var testFiles;

gulp.task('preparetestfiles', function () {
  var bowerDeps = wiredep({
    directory: 'bower_components',
    dependencies: true,
    devDependencies: true
  });

  testFiles = bowerDeps.js.concat([
    'src/main.js',
    'src/**/*.js'
    ]);
});

gulp.task('test', ['preparetestfiles'], function(done) {
  new Server({
    configFile: __dirname + '/karma.conf.js',
    singleRun: true,
    files: testFiles
  }, done).start();
});

gulp.task('partials', function () {
  return gulp.src('src/**/*.html')
    .pipe($.minifyHtml({
      empty: true,
      spare: true,
      quotes: true
    }))
    .pipe($.ngHtml2js({
      moduleName: 'template-partials',
      prefix: '/test-data/src/',
    }))
    .pipe($.rename(function(path){
      path.extname = '.tpl.js';
      return path;
    }))
    .pipe(gulp.dest('src'))
    .pipe($.size());
});

gulp.task('jshint', ['annotate'], function () {
  return gulp.src(['.tmp/**/*.js', '!.tmp/**/*.tpl.js'])
    .pipe($.jshint())
    .pipe($.jshint.reporter('jshint-stylish'))
    .pipe($.jshint.reporter('fail'));
});

gulp.task('prettify-js', function () {
  var options = jsonutil.readFileSync(__dirname + '/.jsbeautifyrc');

  return gulp.src('src/**/*.js')
    .pipe($.jsPrettify(options.js))
    .pipe(gulp.dest('src/'));
});

gulp.task('concat', function() {
  return gulp.src(['.tmp/main.js', '.tmp/**/*.js'])
    .pipe($.concat('ng-test-data.js'))
    .pipe(gulp.dest('./'))
    .pipe($.size());
});

gulp.task('compress', function() {
  return gulp.src(['.tmp/main.js', '.tmp/**/*.js'])
    .pipe($.uglify())
    .pipe($.concat('ng-test-data.min.js'))
    .pipe(gulp.dest('./'))
    .pipe($.size());
});

gulp.task('annotate', function () {
  return gulp.src(['src/**/*.js', '!src/**/*.spec.js'])
    .pipe($.ngAnnotate({add:true, remove: true, single_quotes: true}))
    .pipe(gulp.dest('.tmp'))
    .pipe($.size());
});

gulp.task('clean', function (cb) {
  del(['.tmp'], cb);
});

gulp.task('default', ['build']);

gulp.task('build', function(cb) {
  runSequence('partials', 'test', 'prettify-js', 'jshint', 'concat', 'compress', cb);
});

gulp.task('build:tc', function(cb) {
  runSequence('partials', 'test:tc', 'prettify-js', 'jshint', 'concat', 'compress', cb);
});
