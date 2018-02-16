$(function() {
  $(".dialogBtn").click(function() {
      $("#SendConfirmBtn").attr("href", $(this).attr("href"));
      $("#SendConfirm").modal('toggle');
      return false;
  });
});
$(function() {
  $(".freeItems").click(function() {
      $("#freeItems").modal('toggle');
      return false;
  });
});
$(function() {
  $("#foundations-desktop, #foundations-mobile").click(function() {
      $("#freeFundraisingDataModal").modal('toggle');
          $("#freeFundraisingDataModal .modal-title").removeClass('visible');
          $(".alternativeTitle").addClass('visible');
  });
  return false;
});

$(function(){
  $("#accessCodeConfirm").click(function(){
      $("#accessCodeForm").modal('hide');
      $("#freeFundraisingDataModal").modal('toggle');
      $("#freeFundraisingDataModal .modal-title").addClass('visible');
      $(".alternativeTitle").removeClass('visible');
  })
})

$(function(){
 $("#access-code-link").click(function(){
     $("#accessCodeForm").modal('toggle');
 }) ;
  return false;
});


//scroll
$(function() {
  // Generic selector to be used anywhere
  $(".navbar-link").click(function(e) {

      // Get the href dynamically
      var destination = $(this).attr('href');

      // Prevent href=�#� link from changing the URL hash (optional)
      e.preventDefault();

      // Animate scroll to destination
      $('html, body').animate({
          scrollTop: $(destination).offset().top
      }, 1500);
  });
});


//Get modal title from custom data-attrs titles



$('.dialogBtn').click(function(){

  var d = $(this).data('modal-title');
  $('#myModalLabel').text(d);

  if(d == 'Schedule a Demo') {
      $('.dynamic-element').hide();
  }
  else {
      $('.dynamic-element').show();
  }
});

/*Fade inOut text script*/

$('#fade-text span:gt(0)').hide();
setInterval(function(){
      $('#fade-text span:first-child').fadeOut('slow')
          .next('span')
          .fadeIn('slow')
          .end().appendTo('#fade-text');}, 1000);

/*portfolio click on mobile*/

$('.portfolio-item').bind('click', function(){
  $('.portfolio-item .portfolio-overlay').attr('style', 'display: block !important');
});


//reset carousel slide to first when tab is opened
$('.nav-tabs a').click(function(){
 $('.carousel').carousel(0);
});



$('#pdfModal').on('show.bs.modal', function () {
  //$('.carousel').carousel(0);
  $('.tab-pane').removeClass('active');
  $('#expainer1').addClass('active');
  $('.nav-tabs li').removeClass('active').first().addClass('active');

});
$('#pdfModal').on('show.bs.modal', function () {
  $('.carousel').carousel(0);
});
/*
$('#first-slide-modal').click(function (){
  console.log('a');
  //$('.nav-tabs li').removeClass('active');

})*/

//Autoplay video in modal
$(document).ready(function() {
  $('#youtubeVideo').on('hidden.bs.modal', function() {
      var $this = $(this).find('iframe'),
          tempSrc = $this.attr('src');
      $this.attr('src', "");
      $this.attr('src', tempSrc);
  });

  $('#html5Video').on('hidden.bs.modal', function() {
      var html5Video = document.getElementById("htmlVideo");
      if (html5Video != null) {
          html5Video.pause();
          html5Video.currentTime = 0;
      }
  });

  $('#login-form').submit(function(e){      
      e.preventDefault();
      e.stopPropagation();
      return false;
  }) 
});
