# Project for wsei backend application programming 
___
Topic number 2 - **application for conducting and collecting survey results**
___
The project has already implemented data to improve testing:
## User accounts:
> * ConfirmedUser@example.com
>> User with confirmed address email, can contribute to private status forms.
> * NonConfirmedUser@example.com
>> User with non confirmed address email, cannot contribute to private status forms, but can contribute to `example.com` domain forms.
> * User@different.com
>> User with non confirmed address email, with different domain. Cannot contribute to private status forms, and to `example.com` domain forms, but can contribute to `different.com` domain forms.
> All users have the same password: `Example1!`
## Forms:
> * Job Satisfaction Survey
>> Public status form. Every logged in user can contribute to this form.
> * Product Feedback
>> Private status form. Only users with confirmed address email can contribute to this form.
> * Course Evaluation
>> Domain status form for domain `example.com`.
> * Domain-Specific Feedback
>> Domain status form for domain `different.com`.
___
## Endpoints:

> Users

>> * [GET\] /api/users/login/{userName}/{password}
>> * [GET\] /api/users/{email}
>> * [GET\] /api/users/
>> * [POST\] /api/users/register

> Forms

>> * [GET\] /api/forms/
>> * [GET\] /api/forms/display/{id}
>> * [POST\] /api/forms/create
>> * [PUT\] /api/forms/update
>> * [DELETE\] /api/forms/delete/{id}
>> * [POST\] /api/forms/createquestion/{id}
>> * [PUT\] /api/forms/updatequestion
>> * [DELETE\] /api/forms/deletequestion/{id}
>> * [POST\] /api/forms/answer/{id}
>> * [GET\] /api/forms/responses/{id}
___
Contributors:
- __[Letmii](https://github.com/Letmii)__
- __[wsmolarski](https://github.com/wsmolarski)__
- __[JPrzyborowski](https://github.com/JPrzyborowski)__
