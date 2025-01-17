<?php

namespace App\Http\Requests\UserRequests;

use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Http\Exceptions\HttpResponseException;
use Illuminate\Contracts\Validation\Validator;

class UserCreationRequest extends FormRequest
{
    /**
     * Determine if the user is authorized to make this request.
     *
     * @return bool
     */
    public function authorize()
    {
        return true;
    }

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array<string, mixed>
     */
    public function rules()
    {
        return [
            'Username' => 'required|unique:users|min:6|max:20|alpha_dash',
            'EmailAddress' => 'required|unique:users|email',
            'PwdHash' => 'required|min:8|max:24|confirmed',
            'PwdHash_confirmation' => 'required',
            'Role' => 'integer|min:0|max:2'
        ];
    }

    /*public function failedValidation(Validator $validator)
    {
        throw new HttpResponseException(response()->json($validator->errors(), 422));
    }*/
}
