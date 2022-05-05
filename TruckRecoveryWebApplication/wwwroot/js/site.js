// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let butoon = document.querySelector('.add-work');
butoon.onclick = () => {
   
    let arr = new Array();
    arr.push(document.querySelector('.work-report').value);
    arr.push(' Название кнопки : Добавить работу по ремонту');
    let obj = {};
    obj['отчет по диагностике'] = document.querySelector('.work-report').value;
    obj['Название кнопки'] = 'Добавить работу по ремонту';

    console.log(`${document.querySelector('.work-report').value} Название кнопки : Добавить работу по ремонту`);
    console.log(arr);
    console.log(obj);


    let xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7270/Repairs/Create?OrderId=4");
    xhr.send();
}