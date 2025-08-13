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
    console.log(json);
    todoListViewModel.todos([]);
    
    json.forEach(value => {
        todoListViewModel.todos.push(new todoitemListViewModelFn(value));
    });
    
    todoListViewModel.loading(false);
}

async function updateTodoOrder(){
    const ids = getTodoIds();
    console.log("module updateTodoOrder:");
    console.log(ids);
    await sendTodoIdsToBackend(ids);
    
    const arrayOrder = todoListViewModel.todos.sorted(function(a,b){
        console.log(ids.indexOf(a.id().toString()))
        console.log(ids.indexOf(b.id().toString()))
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
    console.log("module sendTodoIdsToBackend:");
    console.log(data);
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