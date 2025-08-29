function addNewTask(){
    todoListViewModel.todos.push(new todoitemListViewModelFn({id:0,title:''}))
    $("[name=title-todo]").last().focus();
}

async function managerFocusoutTitleTodo(todo){
    const title = todo.title();
    if(!title){
        todoListViewModel.todos.pop();
        return;
    }
    
    const data = JSON.stringify(title);
    const response = await fetch(urlTodos, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        const json = await response.json();
        todo.id(json.id);
    }else{
        // show error message
        handleApiError(response);
    }
}

async function getTodos(){
    todoListViewModel.loading(true);
    
    const response = await fetch(urlTodos, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(!response.ok){
        handleApiError(response);
        return;
    }
    
    const json = await response.json();
    todoListViewModel.todos([]);
    
    json.forEach(value => {
        todoListViewModel.todos.push(new todoitemListViewModelFn(value));
    });
    
    todoListViewModel.loading(false);
}

async function updateTodoOrder(){
    const ids = getTodoIds();
    await sendTodoIdsToBackend(ids);
    
    const arrayOrder = todoListViewModel.todos.sorted(function(a,b){
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });
    
    todoListViewModel.todos([]);
    todoListViewModel.todos(arrayOrder);
}

function getTodoIds(){
    const ids = $("[name=title-todo]").map(function(){
        return $(this).attr("data-id");
    }).get();
    return ids;
}

async function sendTodoIdsToBackend(ids){
    var data = JSON.stringify(ids)
    await fetch(`${urlTodos}/order`,{
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

$(function (){
    $("#reorderable").sortable({
        axis:'y',
        stop:async function(){
            await updateTodoOrder();
        }
    })
})

async function handelClickTodo(todo){
    if(todo.isNew()){
        return;
    }
    
    const response = await fetch(`${urlTodos}/${todo.id()}`,{
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(!response.ok){
        handleApiError(response);
        return;
    }
    
    const json = await response.json();
    
    
    todoEditVM.id = json.id;
    todoEditVM.title(json.title);
    todoEditVM.description(json.description);
    
    todoEditVM.steps([]);
    
    json.steps.forEach(step =>{
        todoEditVM.steps.push(new stepViewModel({...step, isEditing: false}))
        }
    );
    todoEditVM.attachments([]);
    prepareAttachments(json.fileAttachments);
    
    modalTodoEditBootstrap.show();
    
}

async function handleChangeTodoEdit(){
    console.log("Evento change edit");
    const obj ={
        id: todoEditVM.id,
        title: todoEditVM.title(),
        description: todoEditVM.description()
    }
    console.log("todo edit:");
    console.log(obj);
    if(!obj.title)
        return
    
    await editFullTodo(obj);
    const index = todoListViewModel.todos().findIndex(t=>t.id() === obj.id);
    const todo = todoListViewModel.todos()[index];
    todo.title(obj.title);
}

async function editFullTodo(todo){
    const data = JSON.stringify(todo);
    
    const response =await fetch(`${urlTodos}/${todo.id}`,{
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(!response.ok){
        handleApiError(response);
        throw "error";
    }
}

function confirmAction({callBackAcept, callBackCancel, title}){
    Swal.fire({
        title: title || 'Realmente deseas hacer esto?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        focusConfirm:true
    }).then((result)=>{
        if(result.isConfirmed){
            callBackAcept();
        } else if(callBackCancel){
            callBackCancel();
        }
    });
}

function tryDeleteTodo(todo){
    modalTodoEditBootstrap.hide();
    
    confirmAction({
        callBackAcept: () => {
            deleteTodo(todo);
        },
        callbackCancel: () => {
            modalTodoEditBootstrap.show();
        },
        title: `Desea borrar la tarea ${todo.title()}?`
    })
}

async function deleteTodo(todo){
    const idTodo = todo.id;
    const response = await fetch(`${urlTodos}/${idTodo}`,{
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        const index = getIndexTodoEditing();
        todoListViewModel.todos.splice(index,1);
    }
}

function getIndexTodoEditing(){
    return todoListViewModel.todos().findIndex(t=>t.id() == todoEditVM.id);
}

function getTodoEditing(){
    const index = getIndexTodoEditing();
    return todoListViewModel.todos()[index];
}