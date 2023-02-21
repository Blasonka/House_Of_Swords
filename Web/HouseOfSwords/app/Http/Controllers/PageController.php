<?php

namespace App\Http\Controllers;

use App\Http\Requests\LoginRequest;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use App\Http\Controllers\Controller;
use App\Models\Bugreport;
use App\Models\User;
use Exception;

class PageController extends Controller
{
    function index()
    {
        return view('home');
    }

    function about()
    {
        return view('about');
    }

    function download()
    {
        return view('download');
    }

    function bugReport()
    {
        return view('report');
    }

    function register()
    {
        return view('users.register');
    }

    function profil()
    {
        return view('users.profil');
    }

    function verify()
    {
        return view('users.verify');
    }

    function admin()
    {
        $bugs = Bugreport::all();
        return view('admin.index', ['bugs' => $bugs]);
    }

    function owner()
    {
        return view('owner.index');
    }

    function login(LoginRequest $request)
    {
        $PwdSalt =  User::where('Username', $request->Username)->value('PwdSalt');
        $randomChar = [];
        for ($i = 0; $i <= 25; $i++) {
            array_push($randomChar, chr($i + 65));
            array_push($randomChar, chr($i + 97));
        };

        try {
            for ($i = 0; $i <= 50; $i++) {
                $Password = hash('sha512', $request->PwdHash . $PwdSalt . $randomChar[$i]);
                $user = User::where('Username', $request->Username)->where('PwdHash', $Password)->first();
                if ($user) {
                    Auth::login($user);
                    if ($user->IsEmailVerified == 0){ return redirect()->route('verify'); }
                    else{ return redirect()->route('user.profil'); }
                };
            };
            if (!$user){
                return redirect()->back()->withErrors(['PwdHash' => 'Hibás jelszó']);
            }
        } catch (Exception $th) {
            return redirect()->back()->withErrors([$th]);
        }
    }

    function logout(Request $request)
    {
        Auth::logout();
        error_log('logged out');
        $request->session()->invalidate();
        $request->session()->regenerateToken();
        return redirect()->route('index');
    }

    function loginshow()
    {
        return view('users.login');
    }
    function forgottenpw()
    {
        return view('users.forgottenpw');
    }

    function notFound($params)
    {
        return view('404', [$params]);
    }
}
