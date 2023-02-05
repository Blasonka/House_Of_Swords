<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\UserController;
use App\Http\Controllers\TownController;
use App\Http\Controllers\FriendlistController;
use App\Http\Controllers\BuildingController;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:sanctum')->get('/user', function (Request $request) {
    return $request->user();
});

//API működésének tesztje
Route::get('/', function(){
    return [
        'creator' => 'Wauboi',
        'whatIsThis' => 'An API',
        'whatDoesItStandFor' => 'Application Programming Interface',
        'isItWorking' => 'Yes',
        'isItCool' => 'HELL YES'
    ];
});

// users table get method (all & with parameters)
//Route::get('/users', [UserController::class, 'index']);

// towns table get method (all & with parameters)
//Route::get('/towns', [TownController::class, 'index']);

// towns table post method with parameters
//Route::post('/towns/create/{Users_UID}', [TownController::class, 'store']);

// towns table delete method by id
//Route::delete('/towns/{Town_ID}', [TownController::class, 'destroy']);

// friendlist table get method (all & with parameters)
//Route::get('/friendlist', [FriendlistController::class, 'index']);


// TOWNS
Route::resource('towns', TownController::class);
Route::get('/towns/{Town_ID}/buildings', [BuildingController::class, 'showSpecial']);


// USERS
Route::resource('users', UserController::class);
// Route::get('/users', [UserController::class, 'index']);
// Route::post('/users', [UserController::class, 'store']);

// Route::get('/users/{id}', [UserController::class, 'show']);
// Route::patch('/users/{id}', [UserController::class, 'update']);
// Route::delete('/users/{id}', [UserController::class, 'destroy']);


// BUILDINGS
Route::resource('buildings', BuildingController::class);
// Route::get('/buildings', [BuildingController::class, 'index']);
// Route::post('/buildings', [BuildingController::class, 'store']);

// Route::get('/buildings/{id}', [BuildingController::class, 'show']);
// Route::patch('/buildings/{id}', [BuildingController::class, 'update']);
// Route::delete('/buildings/{id}', [BuildingController::class, 'destroy']);


// any unknown methods
Route::any('{params}', function ($params) {
    return 'Error 404: Requested content does not exist → '.$params.'.';
});
